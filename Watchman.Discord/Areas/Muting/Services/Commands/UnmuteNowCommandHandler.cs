using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Services.Factories;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Muting.Services;

namespace Watchman.Discord.Areas.Muting.Services.Commands
{
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
