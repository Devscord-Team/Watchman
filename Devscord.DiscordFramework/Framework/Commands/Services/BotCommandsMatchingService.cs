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
        private readonly Regex _exUserMention = new Regex(@"\<@[^\d]*\d+\>", RegexOptions.Compiled);
        private readonly Regex _exChannelMention = new Regex(@"\<#[^\d]*\d+\>", RegexOptions.Compiled);

        public bool IsMatchedWithCommand(DiscordRequest request, BotCommandTemplate template, bool isCommandCustom)
        {
            if (!request.IsCommandForBot)
            {
                return false;
            }
            if (request.Name.ToLowerInvariant() != template.NormalizedCommandName)
            {
                return false;
            }
            if (isCommandCustom)
            {
                return false;
            }
            return this.CompareArgumentsToProperties(request.Arguments.ToList(), template.Properties.ToList());
        }

        private bool CompareArgumentsToProperties(IReadOnlyCollection<DiscordRequestArgument> arguments, IReadOnlyCollection<BotCommandProperty> properties)
        {
            var notOptionalCount = properties.Count(x => !x.IsOptional);
            var parametersCount = arguments.Count(x => !string.IsNullOrEmpty(x.Name));
            if (parametersCount > properties.Count || parametersCount < notOptionalCount)
            {
                return false;
            }
            foreach (var argument in arguments)
            {
                var matchedByName = properties.FirstOrDefault(property => argument.Name?.ToLowerInvariant() == property.Name.ToLowerInvariant());
                if (matchedByName == null)
                {
                    continue;
                }
                if (matchedByName.Type == BotCommandPropertyType.Bool)
                {
                    continue;
                }
                if (matchedByName.Type == BotCommandPropertyType.List && !string.IsNullOrEmpty(argument.Value))
                {
                    continue;
                }
                if (!this.IsMatchedPropertyType(argument.Value, matchedByName.Type))
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
            if (type == BotCommandPropertyType.Time && !this._exTime.IsMatch(value))
            {
                return false;
            }
            if (type == BotCommandPropertyType.UserMention && !this._exUserMention.IsMatch(value))
            {
                return false;
            }
            if (type == BotCommandPropertyType.ChannelMention && !this._exChannelMention.IsMatch(value))
            {
                return false;
            }
            if (type == BotCommandPropertyType.SingleWord && value.Contains(' '))
            {
                return false;
            }
            return true;
        }
    }
}
