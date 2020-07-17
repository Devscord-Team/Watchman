using Watchman.Cqrs;
using Watchman.DomainModel.Warnings;

namespace Watchman.DomainModel.Warnings.Commands
{
    public class AddWarnEventCommand : ICommand
    {
        public WarnEvent WarnEvent { get; }

        public AddWarnEventCommand(ulong receiverId, ulong granterId, string reason, ulong serverId)
        {
            WarnEvent = new WarnEvent(receiverId, granterId, reason, serverId);
        }
    }
}
