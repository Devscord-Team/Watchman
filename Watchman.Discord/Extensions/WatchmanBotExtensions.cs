using Devscord.DiscordFramework;
using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Administration.Services;
using Watchman.Discord.Areas.Help.Services;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.Discord.Areas.Muting.Services.Commands;
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
                    .AddFromIoC<IResponsesInitService>(responsesService => responsesService.InitNewResponsesFromResources)
                    .AddFromIoC<IConfigurationService>(configurationService => configurationService.InitDefaultConfigurations)
                    .AddFromIoC<ICustomCommandsLoader>(customCommandsLoader => customCommandsLoader.InitDefaultCustomCommands)
                    .AddFromIoC<IHelpDataCollectorService, IHelpDBGeneratorService>((dataCollector, helpService) =>
                        () => helpService.FillDatabase(dataCollector.GetBotCommandsInfo(typeof(WatchmanBot).Assembly)))
                    .AddFromIoC<IInitializationService, IDiscordServersService>((initService, serversService) => async () =>
                    {
                        // when bot was offline for less than 1 minutes, it doesn't make sense to init all servers
                        if (WorkflowBuilder.DisconnectedTimes.LastOrDefault() > DateTime.Now.AddMinutes(-1))
                        {
                            Log.Information("Bot was connected less than 1 minute ago");
                            return;
                        }
                        await serversService.GetDiscordServersAsync().ForEachAwaitAsync(initService.InitServer);
                    })
                    .AddHandler(() => Task.Run(() => Log.Information("Bot has done every Ready tasks.")));
            });
        }

        public static WorkflowBuilder SetOnUserJoined(this WorkflowBuilder builder)
        {
            return builder.AddOnUserJoinedHandlers(builder =>
            {
                builder
                    .AddFromIoC<IWelcomeUserService>(x => x.WelcomeUser)
                    .AddFromIoC<ICommandBus>(commandBus => (contexts) => commandBus.ExecuteAsync(new MuteAgainIfNeededCommand(contexts)));
            });
        }

        public static WorkflowBuilder SetOnDiscordServerAddedBot(this WorkflowBuilder builder)
        {
            return builder.AddOnDiscordServerAddedBotHandlers(builder =>
            {
                builder
                    .AddFromIoC<IInitializationService>(initService => initService.InitServer);
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
                    .AddFromIoC<IMuteRoleInitService>(x => x.InitForChannelAsync);
            });
        }

        public static WorkflowBuilder SetOnChannelRemoved(this WorkflowBuilder builder)
        {
            return builder.AddOnChannelRemovedHandler(builder =>
            {
                builder
                    .AddFromIoC<IComplaintsChannelService>(x => x.RemoveIfNeededComplaintsChannel);
            });
        }

        public static WorkflowBuilder SetOnRoleRemoved(this WorkflowBuilder builder)
        {
            return builder.AddOnRoleRemovedHandlers(builder =>
            {
                builder
                    .AddFromIoC<ITrustRolesService>(x => x.StopTrustingRole);
            });
        }
    }
}
