using System.Collections.Generic;
using Devscord.DiscordFramework.Commons;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserRole
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public ulong ServerId { get; private set; }
        public ICollection<Permission> Permissions { get; private set; }

        public UserRole(ulong id, string name, ulong serverId)
        {
            this.Id = id;
            this.Name = name;
            this.ServerId = serverId;
            this.Permissions = new List<Permission>();
        }

        public UserRole(ulong id, string name, ulong serverId, ICollection<Permission> permissions)
        {
            this.Id = id;
            this.Name = name;
            this.ServerId = serverId;
            this.Permissions = permissions;
        }
    }
}