using Watchman.Cqrs;

namespace Watchman.DomainModel.Responses.Commands
{
    public class RemoveResponseCommand : ICommand
    {
        public ulong ServerId { get; private set; }
        public string OnEvent { get; private set; }

        public RemoveResponseCommand(string onEvent, ulong serverId)
        {
            this.ServerId = serverId;
            this.OnEvent = onEvent;
        }
    }
}