using System;

namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public string RoleName { get; }

        public RoleNotFoundException(string roleName) => this.RoleName = roleName;
    }
}
