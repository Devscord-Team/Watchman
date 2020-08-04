using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Commands
{
    public class UpdateRoleTrustCommand : ICommand
    {
        public ulong RoleId { get; }
        public ulong ServerId { get; }

        public UpdateRoleTrustCommand(ulong roleId, ulong serverId)
        {
            this.RoleId = roleId;
            this.ServerId = serverId;
        }
    }
}
