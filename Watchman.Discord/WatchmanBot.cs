using Autofac;
using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using MongoDB.Driver;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Watchman.Discord.Areas.Help.Services;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.Ioc;
using Watchman.Integrations.Logging;
using Watchman.Integrations.MongoDB;
using Watchman.DomainModel.Settings.Services;
using Watchman.Discord.Integration.DevscordFramework;

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

            return WorkflowBuilder.Create(this._configuration.Token, this._context, typeof(WatchmanBot).Assembly)
                .SetDefaultMiddlewares()
                .AddOnReadyHandlers(builder =>
                {
                    builder
                        .AddHandler(() => Task.Run(() => Log.Information("Bot started and logged in...")))
                        .AddFromIoC<ConfigurationService>(configurationService => configurationService.InitDefaultConfigurations)
                        .AddFromIoC<CustomCommandsLoader>(customCommandsLoader => customCommandsLoader.InitDefaultCustomCommands)
                        .AddFromIoC<HelpDataCollectorService, HelpDBGeneratorService>((dataCollector, helpService) => () =>
                        {
                            Task.Run(() => helpService.FillDatabase(dataCollector.GetCommandsInfo(typeof(WatchmanBot).Assembly)));
                            return Task.CompletedTask;
                        })
                        .AddFromIoC<ResponsesInitService>(responsesService => async () =>
                        {
                            await responsesService.InitNewResponsesFromResources();
                        })
                        .AddFromIoC<InitializationService, DiscordServersService>((initService, serversService) => async () =>
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
                        .AddFromIoC<InitializationService>(initService => async server => await initService.InitServer(server));
                })
                .AddOnWorkflowExceptionHandlers(builder =>
                {
                    builder
                        .AddFromIoC<ExceptionHandlerService>(x => x.LogException)
                        .AddHandler(this.PrintDebugExceptionInfo, onlyOnDebug: true)
                        .AddHandler(this.PrintExceptionOnConsole);
                })
                .AddOnChannelCreatedHandlers(builder =>
                {
                    builder
                        .AddFromIoC<MuteRoleInitService>(x => (_, server) => x.InitForServer(server));
                });
        }

        private void PrintDebugExceptionInfo(Exception e, Contexts contexts)
        {
            var exceptionMessage = this.BuildExceptionMessage(e).ToString();
            var messagesService = this._context.Resolve<MessagesServiceFactory>().Create(contexts);
            messagesService.ChannelId = _configuration.ExceptionChannelID;
            var isBotException = e.InnerException is BotException;
            if (isBotException && _configuration.SendOnlyUnknownExceptionInfo)
            {
                return;
            }
            messagesService.SendMessage(exceptionMessage, Devscord.DiscordFramework.Commons.MessageType.BlockFormatted);
        }

        private void PrintExceptionOnConsole(Exception e, Contexts contexts)
        {
            var exceptionMessage = this.BuildExceptionMessage(e).ToString();
            Console.WriteLine(exceptionMessage);
        }

        private StringBuilder BuildExceptionMessage(Exception e)
        {
            return new StringBuilder($"{e.Message}\r\n\r\n{e.InnerException}\r\n\r\n{e.StackTrace}```").FormatMessageIntoBlock();
        }

        private IContainer GetAutofacContainer(DiscordConfiguration configuration)
        {
            return new ContainerModule(configuration)
                .GetBuilder()
                .Build();
        }
    }
}
