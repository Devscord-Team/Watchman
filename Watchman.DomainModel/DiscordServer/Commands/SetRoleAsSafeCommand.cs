namespace Watchman.DomainModel.DiscordServer.Commands
{
    public class SetRoleAsSafeCommand : UpdateSafetyOfRoleCommand
    {
        public SetRoleAsSafeCommand(ulong roleId, ulong serverId) : base(roleId, serverId)
        {
        }
    }
}
