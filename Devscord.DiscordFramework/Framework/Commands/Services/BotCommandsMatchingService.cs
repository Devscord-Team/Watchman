using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsMatchingService
    {

        private readonly Regex exTime = new Regex(@"\d+(h|m|s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex exMention = new Regex(@"<@&?\d+>", RegexOptions.Compiled);

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

        private bool CompareArgumentsToProperties(List<DiscordRequestArgument> arguments, List<BotCommandProperty> properties)
        {
            if (arguments.Count != properties.Count)
            {
                return false;
            }
            foreach (var argument in arguments)
            {
                var anyIsmatched = properties.Any(property => argument.Name.ToLowerInvariant() == property.Name.ToLowerInvariant() && IsMatchedPropertyType(argument.Value, property.Type));
                if (!anyIsmatched)
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
            else if (type == BotCommandPropertyType.Time && !exTime.IsMatch(value))
            {
                return false;
            }
            else if (type == BotCommandPropertyType.UserMention || type == BotCommandPropertyType.ChannelMention && !exMention.IsMatch(value))
            {
                return false;
            }
            else if (type == BotCommandPropertyType.SingleWord && value.Contains(' '))
            {
                return false;
            }

            return true;
        }
    }
}
