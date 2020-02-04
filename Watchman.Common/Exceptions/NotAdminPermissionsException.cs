using System;

namespace Watchman.Common.Exceptions
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
