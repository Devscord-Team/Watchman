using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Commands
{
    public class AddWarnEventCommand : ICommand
    {
        public ulong ReceiverId { get; }
        public ulong GranterId { get; }
        public string Reason { get; }
        public ulong ServerId { get; }

        public AddWarnEventCommand(ulong receiverId, ulong granterId, string reason, ulong serverId)
        {
            this.ReceiverId = receiverId;
            this.GranterId = granterId;
            this.Reason = reason;
            this.ServerId = serverId;
        }
    }
}
