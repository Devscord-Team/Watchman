using Watchman.Cqrs;

namespace Watchman.DomainModel.Protection.Commands
{
    public class AddComplaintsChannelCommand : ICommand
    {
        public ulong ChannelId { get; }
        public ulong ServerId { get; }

        public AddComplaintsChannelCommand(ulong channelId, ulong serverId)
        {
            this.ChannelId = channelId;
            this.ServerId = serverId;
        }
    }
}
