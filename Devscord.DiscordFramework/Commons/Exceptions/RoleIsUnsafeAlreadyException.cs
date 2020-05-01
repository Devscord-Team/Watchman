using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class RoleIsUnsafeAlreadyException : Exception
    {
        public string RoleName { get; }

        public RoleIsUnsafeAlreadyException(string roleName)
        {
            RoleName = roleName;
        }
    }
}
