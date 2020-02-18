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
            // possible messages:
            // ... -arg val1 val2
            // ... val1 -arg val
            // ... val1 val2 val3
            // ... -arg
            // ... 
            // ... -arg "long value" val2 -arg2

            var arguments = new List<DiscordRequestArgument>();
            var trimmedMess = message.Trim();

            while (trimmedMess.Length > 0)
            {
                var prefix = _possiblePrefixes.FirstOrDefault(x => trimmedMess.StartsWith(x));
                var isStartingWithValue = prefix == null;

                if (isStartingWithValue)
                {
                    var onlyValueArg = GetJustValue(ref trimmedMess);
                    arguments.Add(onlyValueArg);
                    continue;
                }

                var arg = GetOneArg(ref trimmedMess, prefix);
                arguments.Add(arg);
            }
            return arguments;
        }

        private DiscordRequestArgument GetJustValue(ref string trimmedMessage)
        {
            var arg = new DiscordRequestArgument
            {
                Prefix = null,
                Name = null,
                Values = GetValues(trimmedMessage, out var lastValuesIndex)
            };
            trimmedMessage = trimmedMessage.Remove(0, lastValuesIndex).TrimStart();
            return arg;
        }

        private DiscordRequestArgument GetOneArg(ref string trimmedMessage, string prefix)
        {
            var arg = new DiscordRequestArgument { Prefix = prefix };
                    
            trimmedMessage = trimmedMessage.CutStart(prefix);
            var isTheOnlyArg = !trimmedMessage.Contains(' ');

            if (isTheOnlyArg)
            {
                arg.Name = trimmedMessage;
                return arg;
            }

            var lastArgNameIndex = trimmedMessage.IndexOf(' ');
            arg.Name = trimmedMessage[..lastArgNameIndex];

            trimmedMessage = trimmedMessage.CutStart(arg.Name).TrimStart();
            arg.Values = GetValues(trimmedMessage, out var lastValuesIndex);

            trimmedMessage = trimmedMessage.Remove(0, lastValuesIndex).TrimStart();
            return arg;
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
                values = GetMultiValues(valuesSegment, out lastValuesIndex).ToList();
            }
            else
            {
                var value = GetSingleValue(valuesSegment, out lastValuesIndex);
                values.Add(value);
            }
            return values;
        }

        private IEnumerable<string> GetMultiValues(string valuesSegment, out int lastValuesIndex)
        {
            lastValuesIndex = valuesSegment[1..].IndexOf('\"');
            var valuesString = valuesSegment[1..lastValuesIndex];
            return valuesString.Split(' ');
        }

        private string GetSingleValue(string valueSegment, out int lastValuesIndex)
        {
            lastValuesIndex = valueSegment.Contains(' ')
                ? valueSegment.IndexOf(' ')
                : valueSegment.Length;

            var value = valueSegment[..lastValuesIndex];
            return value;
        }
    }
}
