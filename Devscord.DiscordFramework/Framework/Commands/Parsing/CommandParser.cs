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
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return new DiscordRequest { OriginalMessage = original };
            }
            message = message.CutStart(prefix);

            var name = this.GetName(message);
            message = message.CutStart(name);

            var arguments = string.IsNullOrWhiteSpace(message)
                ? new List<DiscordRequestArgument>()
                : this.GetArguments(message);

            return new DiscordRequest
            {
                Prefix = prefix,
                Name = name,
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
            var arguments = new List<DiscordRequestArgument>();
            var trimmedMess = message.Trim();

            for (var i = 0; i < trimmedMess.Length; i++)
            {
                var startsWithArg = _possiblePrefixes.Any(x => trimmedMess[i..].TrimStart().StartsWith(x));
                if (!startsWithArg)
                {
                    var arg = new DiscordRequestArgument
                    {
                        Prefix = null,
                        Name = null,
                        Values = GetValues(trimmedMess[i..], out var lastValuesIndex)
                    };
                    i += lastValuesIndex;
                    arguments.Add(arg);
                    continue;
                }

                foreach (var prefix in _possiblePrefixes)
                {
                    var lastPossiblePrefixIndex = i + prefix.Length;
                    if (lastPossiblePrefixIndex > trimmedMess.Length || prefix != trimmedMess[i..lastPossiblePrefixIndex])
                    {
                        continue;
                    }

                    var arg = new DiscordRequestArgument { Prefix = prefix };

                    // ... --argName value ...
                    // or
                    // 
                    var firstArgNameIndex = lastPossiblePrefixIndex;

                    if (!trimmedMess.Contains(' '))
                    {
                        arg.Name = trimmedMess[firstArgNameIndex..];
                        arguments.Add(arg);

                        i = trimmedMess.Length;
                        break;
                    }

                    var lastArgNameIndex = trimmedMess[firstArgNameIndex..].IndexOf(' ') + firstArgNameIndex;
                    //argName value ...
                    arg.Name = trimmedMess[firstArgNameIndex..lastArgNameIndex];

                    var afterArg = trimmedMess[(lastArgNameIndex + 1)..].TrimStart();
                    arg.Values = GetValues(afterArg, out var lastIndex);
                    arguments.Add(arg);

                    i += lastIndex + 1; // add 1 bcs of space after values
                    if (arg.Name != null)
                    {
                        i += arg.Name.Length + 1; // add 1 bcs of space after name
                    }
                }
            }
            return arguments;
        }

        private IEnumerable<string> GetValues(string valuesSegment, out int lastValuesIndex)
        {
            //value ...
            // or
            //"value val2" ...
            var hasMultiValues = valuesSegment[0] == '\"';

            var values = new List<string>();
            if (hasMultiValues)
            {
                lastValuesIndex = valuesSegment[1..].IndexOf('\"');
                var valuesString = valuesSegment[1..lastValuesIndex];
                valuesString.Split(' ')
                    .ToList()
                    .ForEach(x => values.Add(x));
            }
            else
            {
                lastValuesIndex = valuesSegment.Contains(' ')
                    ? valuesSegment.IndexOf(' ')
                    : valuesSegment.Length - 1;
                
                var value = valuesSegment.Contains(' ') 
                    ? valuesSegment[..lastValuesIndex] 
                    : valuesSegment;

                values.Add(value);
            }
            return values;
        }
    }
}
