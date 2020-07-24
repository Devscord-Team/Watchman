namespace Watchman.DomainModel.DiscordServer.Commands
{
    public class SetRoleAsSafeCommand : UpdateSafetyOfRoleCommand
    {
        public SetRoleAsSafeCommand(string roleName, ulong serverId) : base(roleName, serverId)
        {
        }
    }
}
