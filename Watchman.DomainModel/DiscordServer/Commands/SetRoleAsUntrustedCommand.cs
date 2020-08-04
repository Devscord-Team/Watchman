namespace Watchman.DomainModel.DiscordServer.Commands
{
    public class SetRoleAsUntrustedCommand : UpdateRoleTrustCommand
    {
        public SetRoleAsUntrustedCommand(ulong roleId, ulong serverId) : base(roleId, serverId)
        {
        }
    }
}
