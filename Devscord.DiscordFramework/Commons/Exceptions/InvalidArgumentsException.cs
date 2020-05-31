using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class InvalidArgumentsException : Exception
    {
        public readonly string AvailableArguments;

        public InvalidArgumentsException(string arguments)
        {
            AvailableArguments = arguments;
        }
    }
}
