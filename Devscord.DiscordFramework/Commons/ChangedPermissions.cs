using System.Collections.Generic;

namespace Devscord.DiscordFramework.Commons
{
    public class ChangedPermissions
    {
        public ICollection<Permission> AllowPermissions { get; private set; }
        public ICollection<Permission> DenyPermissions { get; private set; }

        public ChangedPermissions(ICollection<Permission> allowPermissions, ICollection<Permission> denyPermissions)
        {
            this.AllowPermissions = allowPermissions;
            this.DenyPermissions = denyPermissions;
        }
    }
}