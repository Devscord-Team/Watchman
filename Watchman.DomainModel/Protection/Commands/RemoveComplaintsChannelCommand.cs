using Watchman.Cqrs;

namespace Watchman.DomainModel.Protection.Commands
{
    public class RemoveComplaintsChannelCommand : ICommand
    {
        public ComplaintsChannel ComplaintsChannel { get; }

        public RemoveComplaintsChannelCommand(ulong channelId, ulong serverId)
        {
            this.ComplaintsChannel = new ComplaintsChannel(channelId, serverId);
        }

        public RemoveComplaintsChannelCommand(ComplaintsChannel complaintsChannel)
        {
            this.ComplaintsChannel = complaintsChannel;
        }
    }
}
