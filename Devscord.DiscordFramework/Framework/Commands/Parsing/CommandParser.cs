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
        private readonly string[] _possiblePrefixes = { "!", "--", "-", "^", "$", "%" };

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
            return _possiblePrefixes.FirstOrDefault(x => withoutWhitespaces.Any(word => word.StartsWith(x)));
        }

        private string GetName(string message)
        {
            return message.Split(' ').First();
        }

        private IEnumerable<DiscordRequestArgument> GetArguments(string message)
        {
            throw new NotImplementedException();
            var arguments = new List<DiscordRequestArgument>();

            //todo przypadek value przed argumentem
            for (var i = 0; i < message.Length; i++)
            {
                foreach (var prefix in _possiblePrefixes)
                {
                    var lastPossiblePrefixIndex = i + prefix.Length;
                    if (lastPossiblePrefixIndex > message.Length)
                    {
                        continue;
                    }

                    if (prefix == message[i..lastPossiblePrefixIndex])
                    {
                        
                    }
                }
            }

            return arguments;
        }
    }
}
