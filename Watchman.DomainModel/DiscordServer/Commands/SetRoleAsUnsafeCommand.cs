namespace Watchman.DomainModel.DiscordServer.Commands
{
    public class SetRoleAsUnsafeCommand : UpdateSafetyOfRoleCommand
    {
        public SetRoleAsUnsafeCommand(string roleName, ulong serverId) : base(roleName, serverId)
        {
        }
    }
}
