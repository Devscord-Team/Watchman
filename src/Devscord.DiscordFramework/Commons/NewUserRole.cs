using System.Collections.Generic;

namespace Devscord.DiscordFramework.Commons
{
    public class NewUserRole
    {
        public string Name { get; private set; }
        public ICollection<Permission> Permissions { get; private set; }

        public NewUserRole(string name)
        {
            this.Name = name;
            this.Permissions = new List<Permission>();
        }

        public NewUserRole(string name, ICollection<Permission> permissions)
        {
            this.Name = name;
            this.Permissions = permissions;
        }
    }
}
