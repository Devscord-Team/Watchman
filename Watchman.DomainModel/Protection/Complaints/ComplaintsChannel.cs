using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Protection.Complaints
{
    public class ComplaintsChannel : Entity, IAggregateRoot
    {
        public ulong ChannelId { get; private set; }
        public ulong ServerId { get; private set; }

        public ComplaintsChannel(ulong channelId, ulong serverId)
        {
            this.ChannelId = channelId;
            this.ServerId = serverId;
        }
    }
}
