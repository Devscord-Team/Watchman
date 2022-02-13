using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Mutes.Services;

namespace Watchman.Discord.Areas.Protection.Services.Commands
{
    public class UnmuteNowCommand : ICommand
    {
        public Contexts Contexts { get; }
        public UserContext UserToUnmute { get; }

        public UnmuteNowCommand(Contexts contexts, UserContext userToUnmute)
        {
            this.Contexts = contexts;
            this.UserToUnmute = userToUnmute;
        }
    }

    public class UnmuteNowCommandHandler : ICommandHandler<UnmuteNowCommand>
    {
        private readonly ICommandBus commandBus;
        private readonly IMutingService mutingService;
        private readonly IMessagesServiceFactory messagesServiceFactory;

        public UnmuteNowCommandHandler(ICommandBus commandBus, IMutingService mutingService, IMessagesServiceFactory messagesServiceFactory)
        {
            this.commandBus = commandBus;
            this.mutingService = mutingService;
            this.messagesServiceFactory = messagesServiceFactory;
        }

        public async Task HandleAsync(UnmuteNowCommand command)
        {
            var eventToUnmute = this.mutingService.GetNotUnmutedUserMuteEvent(command.Contexts.Server.Id, command.UserToUnmute.Id);
            if (eventToUnmute == null)
            {
                var messagesService = this.messagesServiceFactory.Create(command.Contexts);
                await messagesService.SendResponse(x => x.UserWasntMuted(command.UserToUnmute));
                return;
            }
            await this.commandBus.ExecuteAsync(new UnmuteSpecificEventCommand(command.Contexts, eventToUnmute, command.UserToUnmute));
        }
    }
}
