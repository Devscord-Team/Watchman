using System;

namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class NotAdminPermissionsException : Exception
    {
        public NotAdminPermissionsException() : base("This command needs admin permissions")
        {
        }

        public NotAdminPermissionsException(string message) : base(message)
        {
        }
    }
}
