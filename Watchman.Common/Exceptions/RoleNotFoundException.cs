using System;

namespace Watchman.Common.Exceptions
{
    public class RoleNotFoundException : Exception
    {
        public string RoleName { get; }

        public RoleNotFoundException(string roleName)
        {
            RoleName = roleName;
        }
    }
}
