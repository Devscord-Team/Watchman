using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Administration.Services;
using Watchman.Discord.Areas.Help.Services;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.Integration.DevscordFramework;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Discord.Extensions
{
    public static class WatchmanBotExtensions
    {
        public static WorkflowBuilder SetOnReady(this WorkflowBuilder builder)
        {
            return builder.AddOnReadyHandlers(builder =>
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
            });
        }

        public static WorkflowBuilder SetOnUserJoined(this WorkflowBuilder builder)
        {
            return builder.AddOnUserJoinedHandlers(builder =>
            {
                builder
                    .AddFromIoC<WelcomeUserService>(x => x.WelcomeUser)
                    .AddFromIoC<MutingRejoinedUsersService>(x => x.MuteAgainIfNeeded);
            });
        }

        public static WorkflowBuilder SetOnDiscordServerAddedBot(this WorkflowBuilder builder)
        {
            return builder.AddOnDiscordServerAddedBotHandlers(builder =>
            {
                builder
                    .AddFromIoC<InitializationService>(initService => initService.InitServer);
            });
        }

        public static WorkflowBuilder SetOnWorkflowException(this WorkflowBuilder builder)
        {
            return builder.AddOnWorkflowExceptionHandlers(builder =>
            {
                builder
                    .AddFromIoC<IExceptionHandlerService>(x => (e, r, _) => x.LogException(e))
                    .AddFromIoC<IExceptionHandlerService>(x => x.SendExceptionResponse)
                    .AddFromIoC<IExceptionHandlerService>(x => (e, r, c) => x.PrintDebugExceptionInfo(e, c), onlyOnDebug: true)
                    .AddFromIoC<IExceptionHandlerService>(x => (e, r, _) => x.SendExceptionToDebugServer(e));
            });
        }

        public static WorkflowBuilder SetOnChannelCreated(this WorkflowBuilder builder)
        {
            return builder.AddOnChannelCreatedHandlers(builder =>
            {
                builder
                    .AddFromIoC<MuteRoleInitService>(x => x.InitForChannelAsync);
            });
        }

        public static WorkflowBuilder SetOnChannelRemoved(this WorkflowBuilder builder)
        {
            return builder.AddOnChannelRemovedHandler(builder =>
            {
                builder
                    .AddFromIoC<ComplaintsChannelService>(x => x.RemoveIfNeededComplaintsChannel);
            });
        }

        public static WorkflowBuilder SetOnRoleRemoved(this WorkflowBuilder builder)
        {
            return builder.AddOnRoleRemovedHandlers(builder =>
            {
                builder
                    .AddFromIoC<TrustRolesService>(x => x.StopTrustingRole);
            });
        }
    }
}
