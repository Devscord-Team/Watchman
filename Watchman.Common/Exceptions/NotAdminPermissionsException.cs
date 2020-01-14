using System;

namespace Devscord.DiscordFramework
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
