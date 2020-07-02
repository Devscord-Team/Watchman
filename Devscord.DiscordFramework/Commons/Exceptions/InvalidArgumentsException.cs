using System;

namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class InvalidArgumentsException : Exception
    {
        public string AvailableArguments { get; }

        public InvalidArgumentsException(string arguments)
        {
            this.AvailableArguments = arguments;
        }
    }
}
