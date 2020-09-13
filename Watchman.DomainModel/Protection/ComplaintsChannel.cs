using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Protection
{
    public class ComplaintsChannel : Entity, IAggregateRoot
    {
        public ulong ChannelId { get; set; }

        public ComplaintsChannel(ulong channelId)
        {
            this.ChannelId = channelId;
        }
    }
}
