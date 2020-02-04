using System;

namespace Watchman.Common.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public string Mention { get; }

        public UserNotFoundException(string mention)
        {
            this.Mention = mention;
        }
    }
}
