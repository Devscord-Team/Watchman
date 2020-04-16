using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Commands
{
    public abstract class UpdateSafetyOfRoleCommand : ICommand
    {
        public string RoleName { get; }
        public ulong ServerId { get; }

        protected UpdateSafetyOfRoleCommand(string roleName, ulong serverId)
        {
            RoleName = roleName;
            ServerId = serverId;
        }
    }
}
