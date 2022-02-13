using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Mutes.Services;

namespace Watchman.Discord.Areas.Protection.Services.Commands
{
    public class MuteAgainIfNeededCommandHandler : ICommandHandler<MuteAgainIfNeededCommand>
    {
        private readonly ICommandBus commandBus;
        private readonly IMutingService mutingService;

        public MuteAgainIfNeededCommandHandler(ICommandBus commandBus, IMutingService mutingService)
        {
            this.commandBus = commandBus;
            this.mutingService = mutingService;
        }

        public async Task HandleAsync(MuteAgainIfNeededCommand command)
        {
            var notUnmuted = this.mutingService.GetNotUnmutedUserMuteEvent(command.Contexts.Server.Id, command.Contexts.User.Id);
            if (notUnmuted != null)
            {
                await this.commandBus.ExecuteAsync(new MuteUserOrOverwriteCommand(command.Contexts, notUnmuted, command.Contexts.User));
            }
        }
    }
}
