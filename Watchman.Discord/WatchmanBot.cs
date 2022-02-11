using Autofac;
using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Services;
using MongoDB.Driver;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Help.Services;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.Ioc;
using Watchman.Integrations.Logging;
using Watchman.Discord.Integration.DevscordFramework;
using Watchman.Integrations.Database.MongoDB;
using Watchman.DomainModel.Configuration;
using Watchman.DomainModel.Configuration.Services;
using Watchman.Discord.Areas.Administration.Services;

namespace Watchman.Discord
{
    public class WatchmanBot
    {
        private readonly DiscordConfiguration _configuration;
        private readonly IComponentContext _context;

        public WatchmanBot(DiscordConfiguration configuration, IComponentContext context = null)
        {
            this._configuration = configuration;
            this._context = context ?? this.GetAutofacContainer(configuration).Resolve<IComponentContext>();
            Log.Logger = SerilogInitializer.Initialize(this._context.Resolve<IMongoDatabase>());
            Log.Information("Bot created...");
        }

        public WorkflowBuilder GetWorkflowBuilder()
        {
            MongoConfiguration.Initialize();
            ExceptionHandlerService.DiscordConfiguration = this._configuration; //todo ioc

            return WorkflowBuilder.Create(this._configuration.Token, this._context.Resolve<IWorkflow>())
                .SetDefaultMiddlewares()
                .AddOnReadyHandlers(builder =>
                {
                    builder
                        .AddHandler(() => Task.Run(() => Log.Information("Bot started and logged in...")))
                        .AddFromIoC<ConfigurationService>(configurationService => configurationService.InitDefaultConfigurations)
                        .AddFromIoC<CustomCommandsLoader>(customCommandsLoader => customCommandsLoader.InitDefaultCustomCommands)
                        .AddFromIoC<IHelpDataCollectorService, HelpDBGeneratorService>((dataCollector, helpService) =>
                            () => helpService.FillDatabase(dataCollector.GetBotCommandsInfo(typeof(WatchmanBot).Assembly)))
                        .AddFromIoC<ResponsesInitService>(responsesService => responsesService.InitNewResponsesFromResources)
                        .AddFromIoC<InitializationService, IDiscordServersService>((initService, serversService) => async () =>
                        {
                            var stopwatch = Stopwatch.StartNew();
                            // when bot was offline for less than 1 minutes, it doesn't make sense to init all servers
                            if (WorkflowBuilder.DisconnectedTimes.LastOrDefault() > DateTime.Now.AddMinutes(-1))
                            {
                                Log.Information("Bot was connected less than 1 minute ago");
                                return;
                            }
                            await serversService.GetDiscordServersAsync().ForEachAwaitAsync(initService.InitServer);
                            Log.Information(stopwatch.ElapsedMilliseconds.ToString());
                        })
                        .AddHandler(() => Task.Run(() => Log.Information("Bot has done every Ready tasks.")));
                })
                .AddOnUserJoinedHandlers(builder =>
                {
                    builder
                        .AddFromIoC<WelcomeUserService>(x => x.WelcomeUser)
                        .AddFromIoC<MutingRejoinedUsersService>(x => x.MuteAgainIfNeeded);
                })
                .AddOnDiscordServerAddedBotHandlers(builder =>
                {
                    builder
                        .AddFromIoC<InitializationService>(initService => initService.InitServer);
                })
                .AddOnWorkflowExceptionHandlers(builder =>
                {
                    builder
                        .AddFromIoC<IExceptionHandlerService>(x => (e, r, _) => x.LogException(e))
                        .AddFromIoC<IExceptionHandlerService>(x => x.SendExceptionResponse)
                        .AddFromIoC<IExceptionHandlerService>(x => (e, r, c) =>  x.PrintDebugExceptionInfo(e, c), onlyOnDebug: true)
                        .AddFromIoC<IExceptionHandlerService>(x => (e, r, _) => x.SendExceptionToDebugServer(e));
                })
                .AddOnChannelCreatedHandlers(builder =>
                {
                    builder
                        .AddFromIoC<MuteRoleInitService>(x => x.InitForChannelAsync);
                })
                .AddOnChannelRemovedHandler(builder =>
                {
                    builder
                        .AddFromIoC<ComplaintsChannelService>(x => x.RemoveIfNeededComplaintsChannel);
                })
                .AddOnRoleRemovedHandlers(builder =>
                {
                    builder
                        .AddFromIoC<TrustRolesService>(x => x.StopTrustingRole);
                });
        }

        private IContainer GetAutofacContainer(DiscordConfiguration configuration)
        {
            return new ContainerModule(configuration)
                .GetBuilder()
                .Build();
        }
    }
}
