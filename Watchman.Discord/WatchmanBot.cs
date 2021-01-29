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
using Watchman.Discord.Areas.Wallet.Services;
using System.Collections.Generic;
using Watchman.DomainModel.Wallet.ValueObjects;

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
            ExceptionHandlerService.DiscordConfiguration = this._configuration;

            return WorkflowBuilder.Create(this._configuration.Token, this._context, typeof(WatchmanBot).Assembly)
                .SetDefaultMiddlewares()
                .AddOnReadyHandlers(builder =>
                {
                    builder
                        .AddHandler(() => Task.Run(() => Log.Information("Bot started and logged in...")))
                        .AddFromIoC<ConfigurationService>(configurationService => configurationService.InitDefaultConfigurations)
                        .AddFromIoC<CustomCommandsLoader>(customCommandsLoader => customCommandsLoader.InitDefaultCustomCommands)
                        .AddFromIoC<HelpDataCollectorService, HelpDBGeneratorService>((dataCollector, helpService) =>
                            () => helpService.FillDatabase(dataCollector.GetBotCommandsInfo(typeof(WatchmanBot).Assembly)))
                        .AddFromIoC<ResponsesInitService>(responsesService => responsesService.InitNewResponsesFromResources)
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
                        .AddFromIoC<WalletsInitializationService, DiscordServersService>((walletsInitService, serversService) => 
                            () => serversService.GetDiscordServersAsync().ForEachAwaitAsync(server => walletsInitService.TryCreateServerWalletForServer(server.Id)))
                        .AddFromIoC<WalletsInitializationService, DiscordServersService>((walletsInitService, serversService) =>
                            () => serversService.GetDiscordServersAsync().ForEachAwaitAsync(server => this.InitializeWalletForUsersOnServer(walletsInitService, serversService, server.Id)))
                        .AddHandler(() => Task.Run(() => Log.Information("Bot has done every Ready tasks.")));
                })
                .AddOnUserJoinedHandlers(builder =>
                {
                    builder
                        .AddFromIoC<WelcomeUserService>(x => x.WelcomeUser)
                        .AddFromIoC<MutingRejoinedUsersService>(x => x.MuteAgainIfNeeded)
                        .AddFromIoC<WalletsInitializationService>(x => (contexts) => x.TryCreateServerWalletForUser(contexts.Server.Id, contexts.User.Id));

                })
                .AddOnDiscordServerAddedBotHandlers(builder =>
                {
                    builder
                        .AddFromIoC<InitializationService>(initService => initService.InitServer)
                        .AddFromIoC<WalletsInitializationService, DiscordServersService>((walletsInitService, serversService) =>
                            (serverContext) => serversService.GetDiscordServersAsync().ForEachAwaitAsync(server => walletsInitService.TryCreateServerWalletForServer(serverContext.Id)))
                        .AddFromIoC<WalletsInitializationService, DiscordServersService>((walletsInitService, serversService) => (serverContext) => this.InitializeWalletForUsersOnServer(walletsInitService, serversService, serverContext.Id));
                })
                .AddOnWorkflowExceptionHandlers(builder =>
                {
                    builder
                        .AddFromIoC<ExceptionHandlerService>(x => (e, _) => x.LogException(e))
                        .AddFromIoC<ExceptionHandlerService>(x => x.SendExceptionResponse)
                        .AddFromIoC<ExceptionHandlerService>(x => x.PrintDebugExceptionInfo, onlyOnDebug: true)
                        .AddFromIoC<ExceptionHandlerService>(x => (e, _) => x.SendExceptionToDebugServer(e));
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

        private async Task InitializeWalletForUsersOnServer(WalletsInitializationService walletsInitService, DiscordServersService serversService, ulong serverId)
        {
            var usersOnServer = await serversService.GetUsersIdsFromServer(serverId);
            var tasks = new List<Task>();
            foreach (var userId in usersOnServer)
            {
                if(userId == WalletTransaction.DEVSCORD_TEAM_TRANSACTION_USER_ID)
                {
                    continue;
                }
                var task = walletsInitService.TryCreateServerWalletForUser(serverId, userId);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        private IContainer GetAutofacContainer(DiscordConfiguration configuration)
        {
            return new ContainerModule(configuration)
                .GetBuilder()
                .Build();
        }
    }
}
