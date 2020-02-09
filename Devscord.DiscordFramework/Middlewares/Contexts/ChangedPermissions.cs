using System.Collections.Generic;
using Watchman.Common.Models;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class ChangedPermissions
    {
        public Permissions AllowPermissions { get; private set; }
        public Permissions DenyPermissions { get; private set; }

        public ChangedPermissions(IEnumerable<Permission> allowPermissions, IEnumerable<Permission> denyPermissions)
        {
            AllowPermissions = new Permissions(allowPermissions);
            DenyPermissions = new Permissions(denyPermissions);
        }
    }
}