namespace Watchman.DomainModel.DiscordServer.Commands
{
    public class SetRoleAsUnsafeCommand : UpdateSafetyOfRoleCommand
    {
        public SetRoleAsUnsafeCommand(ulong roleId, ulong serverId) : base(roleId, serverId)
        {
        }
    }
}
