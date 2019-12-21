using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands.Parsing
{
    public class CommandParser
    {
        private readonly string[] possiblePrefixes = new string[] { "!", "--", "-", "^", "$", "%" };

        public DiscordRequest Parse(string message)
        {
            var prefix = this.GetPrefix(message);
            message = message.CutStart(prefix);

            var name = this.GetName(message);
            message = message.CutStart(name);

            var arguments = this.GetArguments(message);

            return new DiscordRequest
            {
                Prefix = prefix,
                Name = name,
                ArgumentsPrefix = arguments?.FirstOrDefault()?.Prefix,
                Arguments = arguments
            };
        }

        private string GetPrefix(string message)
        {
            return possiblePrefixes.FirstOrDefault(x => message.StartsWith(x));
        }

        private string GetName(string message)
        {
            return message.Split(' ').First();
        }

        private IEnumerable<DiscordRequestArgument> GetArguments(string message)
        {
            return default;
        }
    }
}
