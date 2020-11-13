using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Commands
{
    public abstract class UpdateSafetyOfRoleCommand : ICommand
    {
        public ulong RoleId { get; }
        public ulong ServerId { get; }

        protected UpdateSafetyOfRoleCommand(ulong roleName, ulong serverId)
        {
            this.RoleId = roleName;
            this.ServerId = serverId;
        }
    }
}
