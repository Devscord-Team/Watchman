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
            var original = (string)message.Clone();
            var prefix = this.GetPrefix(message);
            if(string.IsNullOrWhiteSpace(prefix))
            {
                return new DiscordRequest { OriginalMessage = original };
            }
            message = message.CutStart(prefix);

            var name = this.GetName(message);
            message = message.CutStart(name);

            var arguments = this.GetArguments(message);
            return new DiscordRequest
            {
                Prefix = prefix,
                Name = name,
                ArgumentsPrefix = arguments?.FirstOrDefault()?.Prefix,
                Arguments = arguments,
                OriginalMessage = original
            };
        }

        private string GetPrefix(string message)
        {
            var withoutWhitespaces = message.Trim().Split(' ');
            return possiblePrefixes.FirstOrDefault(x => withoutWhitespaces.Any(word => word.StartsWith(x)));
        }

        private string GetName(string message)
        {
            return message.Split(' ').First();
        }

        private IEnumerable<DiscordRequestArgument> GetArguments(string message)
        {
            var prefix = this.GetPrefix(message);
            var splitted = message.Split(prefix)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim());
            return splitted.Select(x => this.GetArgument(x, prefix));
        }

        private DiscordRequestArgument GetArgument(string message, string prefix)
        {
            var isParameter = message.Split().Length > 1;
            var splitted = message.Split(' ');
            var parameter = isParameter ? splitted.First() : null;
            var values = splitted.Skip(isParameter ? 1 : 0);

            return new DiscordRequestArgument
            {
                Name = parameter,
                Prefix = prefix,
                Values = values
            };
        }
    }
}
