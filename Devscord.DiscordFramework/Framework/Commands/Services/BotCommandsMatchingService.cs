using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsMatchingService
    {

        private readonly Regex _exTime = new Regex(@"\d+(h|m|s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex _exMention = new Regex(@"<@&?\d+>", RegexOptions.Compiled);

        public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template)
        {
            if (!request.IsCommandForBot)
            {
                return false;
            }
            if (request.Name.ToLowerInvariant() != template.NormalizedCommandName)
            {
                return false;
            }
            if (!CompareArgumentsToProperties(request.Arguments.ToList(), template.Properties.ToList()))
            {
                return false;
            }
            return true;
        }

        private bool CompareArgumentsToProperties(IReadOnlyCollection<DiscordRequestArgument> arguments, IReadOnlyCollection<BotCommandProperty> properties)
        {
            var notOptionalCount = properties.Count(x => !x.IsOptional);
            if (arguments.Count > properties.Count || arguments.Count < notOptionalCount)
            {
                return false;
            }
            foreach (var argument in arguments)
            {
                var anyIsMatched = properties.Any(property => argument.Name?.ToLowerInvariant() == property.Name.ToLowerInvariant() && IsMatchedPropertyType(argument.Value, property.Type));
                if (!anyIsMatched)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsMatchedPropertyType(string value, BotCommandPropertyType type)
        {
            if (type == BotCommandPropertyType.Number && !int.TryParse(value, out _))
            {
                return false;
            }
            if (type == BotCommandPropertyType.Time && !_exTime.IsMatch(value))
            {
                return false;
            }
            if (type == BotCommandPropertyType.UserMention || type == BotCommandPropertyType.ChannelMention && !_exMention.IsMatch(value))
            {
                return false;
            }
            if (type == BotCommandPropertyType.SingleWord && value.Contains(' '))
            {
                return false;
            }
            if (type == BotCommandPropertyType.Bool && !bool.TryParse(value, out _))
            {
                return false;
            }
            return true;
        }
    }
}
