using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services;
using Watchman.Integrations.MongoDB;
using Watchman.Discord.Ioc;
using Devscord.DiscordFramework;
using Autofac;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Help.Services;
using System.Text;
using Devscord.DiscordFramework.Commons.Extensions;
using System.Linq;
using Watchman.Integrations.Logging;
using MongoDB.Driver;
using Serilog;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.Discord.Areas.Users.Services;
using System.Diagnostics;

namespace Watchman.Discord
{
    public class WatchmanBot
    {
        private readonly DiscordConfiguration _configuration;
        private readonly IComponentContext _context;

        public WatchmanBot(DiscordConfiguration configuration, IComponentContext context = null)
        {
            this._configuration = configuration;
            this._context = context ?? GetAutofacContainer(configuration).Resolve<IComponentContext>();
            Log.Logger = SerilogInitializer.Initialize(this._context.Resolve<IMongoDatabase>());
            Log.Information("Bot created...");
        }

        public WorkflowBuilder GetWorkflowBuilder()
        {
            MongoConfiguration.Initialize();

            return WorkflowBuilder.Create(_configuration.Token, this._context, typeof(WatchmanBot).Assembly)
                .SetDefaultMiddlewares()
                .AddOnReadyHandlers(builder =>
                {
                    builder
                        .AddHandler(() => Task.Run(() => Log.Information("Bot started and logged in...")))
                        .AddFromIoC<HelpDataCollectorService, HelpDBGeneratorService>((dataCollector, helpService) => () =>
                        {
                            Task.Run(() => helpService.FillDatabase(dataCollector.GetCommandsInfo(typeof(WatchmanBot).Assembly)));
                            return Task.CompletedTask;
                        })
                        .AddFromIoC<UnmutingExpiredMuteEventsService, DiscordServersService>((unmutingService, serversService) => async () =>
                        {
                            var servers = (await serversService.GetDiscordServers()).ToList();
                            servers.ForEach(unmutingService.UnmuteUsersInit);
                        })
                        .AddFromIoC<CyclicStatisticsGeneratorService>(cyclicStatsGenerator => () =>
                        {
                            cyclicStatsGenerator.StartCyclicCaching();
                            return Task.CompletedTask;
                        })
                        .AddFromIoC<ResponsesInitService>(responsesService => async () =>
                        {
                            await responsesService.InitNewResponsesFromResources();
                        })
                        .AddFromIoC<InitializationService, DiscordServersService>((initService, serversService) => () =>
                        {
                            var stopwatch = Stopwatch.StartNew();

                            // when bot was offline for less than 5 minutes, it doesn't make sense to init all servers
                            if (WorkflowBuilder.DisconnectedTimes.LastOrDefault() > DateTime.Now.AddMinutes(-5))
                            {
                                Log.Information("Bot was connected less than 5 minutes ago");
                                return Task.CompletedTask;
                            }
                            var servers = serversService.GetDiscordServers().Result;
                            Task.WaitAll(servers.Select(async server =>
                            {
                                Log.Information($"Initializing server: {server.Name}");
                                await initService.InitServer(server);
                                Log.Information($"Done server: {server.Name}");
                            }).ToArray());

                            Log.Information(stopwatch.ElapsedMilliseconds.ToString());
                            return Task.CompletedTask;
                        })
                        .AddHandler(() => Task.Run(() => Log.Information("Bot has done every Ready tasks.")));
                })
                .AddOnUserJoinedHandlers(builder =>
                {
                    builder
                        .AddFromIoC<WelcomeUserService>(x => x.WelcomeUser)
                        .AddFromIoC<MutingRejoinedUsersService>(x => x.MuteAgainIfNeeded);
                })
                .AddOnDiscordServerAddedBot(builder =>
                {
                    builder
                        .AddFromIoC<InitializationService>(initService => initService.InitServer);
                })
                .AddOnWorkflowExceptionHandlers(builder =>
                {
                    builder
                        .AddFromIoC<ExceptionHandlerService>(x => x.LogException)
                        .AddHandler(this.PrintDebugExceptionInfo, onlyOnDebug: true)
                        .AddHandler(this.PrintExceptionOnConsole);
                })
                .Build();
        }

        private void PrintDebugExceptionInfo(Exception e, Contexts contexts)
        {
            var exceptionMessage = BuildExceptionMessage(e).ToString();
            var messagesService = _context.Resolve<MessagesServiceFactory>().Create(contexts);
            messagesService.SendMessage(exceptionMessage, Devscord.DiscordFramework.Commons.MessageType.BlockFormatted);
        }

        private void PrintExceptionOnConsole(Exception e, Contexts contexts)
        {
            var exceptionMessage = BuildExceptionMessage(e).ToString();
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
