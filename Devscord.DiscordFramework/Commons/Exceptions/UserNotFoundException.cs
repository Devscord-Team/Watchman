using System;

namespace Devscord.DiscordFramework.Commons.Exceptions
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
