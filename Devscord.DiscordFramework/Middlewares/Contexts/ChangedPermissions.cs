using System.Collections.Generic;
using Devscord.DiscordFramework.Commons;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class ChangedPermissions
    {
        public Permissions AllowPermissions { get; private set; }
        public Permissions DenyPermissions { get; private set; }

        public ChangedPermissions(ICollection<Permission> allowPermissions, ICollection<Permission> denyPermissions)
        {
            AllowPermissions = new Permissions(allowPermissions);
            DenyPermissions = new Permissions(denyPermissions);
        }
    }
}