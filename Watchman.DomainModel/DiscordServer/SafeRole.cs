using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.DiscordServer
{
    public class SafeRole : Entity
    {
        public ulong RoleId { get; private set; }
        public ulong ServerId { get; private set; }

        public SafeRole(ulong roleId, ulong serverId)
        {
            this.RoleId = roleId;
            this.ServerId = serverId;
        }
    }
}
