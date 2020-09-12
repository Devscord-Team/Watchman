using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Commands
{
    public class AddWarnEventCommand : ICommand
    {
        public ulong GrantorId { get; }
        public ulong ReceiverId { get; }
        public string Reason { get; }
        public ulong ServerId { get; }

        public AddWarnEventCommand(ulong grantorId, ulong receiverId, string reason, ulong serverId)
        {
            this.GrantorId = grantorId;
            this.ReceiverId = receiverId;
            this.Reason = reason;
            this.ServerId = serverId;
        }
    }
}
