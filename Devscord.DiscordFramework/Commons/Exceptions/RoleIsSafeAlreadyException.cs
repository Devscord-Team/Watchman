using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class RoleIsSafeAlreadyException : Exception
    {
        public string RoleName { get; }

        public RoleIsSafeAlreadyException(string roleName)
        {
            RoleName = roleName;
        }
    }
}
