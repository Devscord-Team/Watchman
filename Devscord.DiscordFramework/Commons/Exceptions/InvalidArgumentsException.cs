using System;
using System.Collections.Generic;
using System.Text;

namespace Devscord.DiscordFramework.Commons.Exceptions
{
    public class InvalidArgumentsException : Exception
    {
        public InvalidArgumentsException()
        {
        }

        public InvalidArgumentsException(string message) : base(message) 
        {
        }
    }
}
