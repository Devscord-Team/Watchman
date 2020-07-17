using Watchman.Cqrs;

namespace Watchman.DomainModel.Warns.Commands
{
    public class AddWarnEventCommand : ICommand
    {
        public WarnEvent WarnEvent { get; }

        public AddWarnEventCommand(ulong receiverId, ulong granterId, string reason, ulong serverId)
        {
            this.WarnEvent = new WarnEvent(receiverId, granterId, reason, serverId);
        }
    }
}
