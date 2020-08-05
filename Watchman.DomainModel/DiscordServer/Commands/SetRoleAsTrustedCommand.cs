using Watchman.Cqrs;

namespace Watchman.DomainModel.DiscordServer.Commands
{
    public class SetRoleAsTrustedCommand : UpdateRoleTrustCommand
    {
        public SetRoleAsTrustedCommand(ulong roleId, ulong serverId) : base(roleId, serverId)
        {
        }
    }
}
