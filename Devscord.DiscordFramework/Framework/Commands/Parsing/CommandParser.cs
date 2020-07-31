using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Framework.Commands.Parsing
{
    public class CommandParser
    {
        private readonly string[] _possiblePrefixes = { "!", "--", "-", "^", "$", "%" };

        public DiscordRequest Parse(string message, DateTime sentAt)
        {
            var original = (string) message.Clone();
            var prefix = this.GetPrefix(message);
            if (string.IsNullOrWhiteSpace(prefix))
            {
                return new DiscordRequest { OriginalMessage = original, SentAt = sentAt };
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
                OriginalMessage = original,
                SentAt = sentAt
            };
        }

        private string GetPrefix(string message)
        {
            var withoutWhitespaces = message.Trim();
            return this._possiblePrefixes.FirstOrDefault(x => withoutWhitespaces.StartsWith(x));
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
                var prefix = this.GetPrefix(trimmedMess);
                var isStartingWithValue = prefix == null;

                if (isStartingWithValue)
                {
                    var onlyValueArg = this.GetJustValue(ref trimmedMess);
                    arguments.Add(onlyValueArg);
                    continue;
                }

                var arg = this.GetOneArg(ref trimmedMess, prefix);
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
                Value = this.GetValue(trimmedMessage, out var nextIndexAfterValue)
            };
            trimmedMessage = trimmedMessage.Remove(0, nextIndexAfterValue).TrimStart();
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
                trimmedMessage = string.Empty;
                return arg;
            }

            var lastArgNameIndex = trimmedMessage.IndexOf(' ');
            arg.Name = trimmedMessage[..lastArgNameIndex];

            trimmedMessage = trimmedMessage.CutStart(arg.Name).TrimStart();
            arg.Value = this.GetValue(trimmedMessage, out var nextIndexAfterValue);

            trimmedMessage = trimmedMessage[nextIndexAfterValue..].TrimStart();
            return arg;
        }

        private string GetValue(string valuesSegment, out int nextIndexAfterValue)
        {
            //value ...
            // or
            //"value val2" ...
            var isLongValue = valuesSegment[0] == '\"';

            return isLongValue
                ? this.GetLongValue(valuesSegment, out nextIndexAfterValue)
                : this.GetSingleValue(valuesSegment, out nextIndexAfterValue);
        }

        private string GetLongValue(string valuesSegment, out int nextIndexAfterValue)
        {
            var lastValuesIndex = valuesSegment[1..].IndexOf('\"') + 1; // adding 1 bcs [1..]
            if (lastValuesIndex == 0)
            {
                throw new InvalidArgumentsException();
            }
            var valueString = valuesSegment[1..lastValuesIndex];
            nextIndexAfterValue = lastValuesIndex + 1;
            return valueString;
        }

        private string GetSingleValue(string valueSegment, out int nextIndexAfterValue)
        {
            nextIndexAfterValue = valueSegment.Contains(' ')
                ? valueSegment.IndexOf(' ')
                : valueSegment.Length;

            var value = valueSegment[..nextIndexAfterValue];
            return value;
        }
    }
}
