using System.Collections.Generic;
using Devscord.DiscordFramework.Commons;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class UserRole
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public ICollection<Permission> Permissions { get; private set; }

        public UserRole(string name)
        {
            this.Name = name;
            this.Permissions = new List<Permission>();
        }

        public UserRole(string name, ICollection<Permission> permissions)
        {
            this.Name = name;
            this.Permissions = permissions;
        }

        public UserRole(ulong id, string name, ICollection<Permission> permissions)
        {
            this.Id = id;
            this.Name = name;
            this.Permissions = permissions;
        }
    }
}