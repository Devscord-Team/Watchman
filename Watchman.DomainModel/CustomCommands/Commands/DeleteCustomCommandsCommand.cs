using Watchman.Cqrs;

namespace Watchman.DomainModel.CustomCommands.Commands
{
    public class DeleteCustomCommandsCommand : ICommand
    {
        public string CommandFullName { get; private set; }
        public ulong ServerId { get; private set; }

        public DeleteCustomCommandsCommand(string commandFullName, ulong serverId)
        {
            CommandFullName = commandFullName;
            ServerId = serverId;
        }
    }
}
