using Watchman.Cqrs;

namespace Watchman.DomainModel.Protection.Commands
{
    public class AddComplaintsChannelCommand : ICommand
    {
        public ulong ChannelId { get; }

        public AddComplaintsChannelCommand(ulong channelId)
        {
            this.ChannelId = channelId;
        }
    }
}
