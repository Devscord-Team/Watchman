using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsMatchingService
    {
        private readonly Regex _exTime = new Regex(@"\d+(ms|d|h|m|s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex _exUserMention = new Regex(@"\<@!\d+\>", RegexOptions.Compiled);
        private readonly Regex _exChannelMention = new Regex(@"\<#\d+\>", RegexOptions.Compiled);

        public bool IsNormalCommand(DiscordRequest request, BotCommandTemplate template)
        {
            if (!request.IsCommandForBot)
            {
                return false;
            }
            return request.Name.ToLowerInvariant() == template.NormalizedCommandName;
        }

        public bool IsMatchedWithNormalCommand(BotCommandTemplate template, IEnumerable<DiscordRequestArgument> arguments)
        {  
            var requiredProperties = template.Properties.Where(x => !x.IsOptional);
            if (requiredProperties.Any(property => !arguments.Any(arg => property.Name.ToLowerInvariant() == arg.Name?.ToLowerInvariant())))
            {
                throw new NotEnoughArgumentsException();
            }
            var argsAndValues = arguments.Select(arg => new KeyValuePair<string, string>(arg.Name, arg.Value)).ToList();

            return ComparePropertiesToArgsAndValues(template.Properties, argsAndValues);
        }

        public bool IsMatchedWithCustomCommand(BotCommandTemplate template, Regex customTemplate, string input)
        {
            var matchGroups = customTemplate.Match(input).Groups;
            var requiredProperties = template.Properties.Where(x => !x.IsOptional);
            if (requiredProperties.Any(x => !matchGroups.ContainsKey(x.Name)))
            {
                Log.Warning("Custom template {customTemplate} is not valid for {commandName}", customTemplate, template.CommandName);
                throw new InvalidArgumentsException();
            }
            var argsAndValues = matchGroups.Keys
                .Select(arg => new KeyValuePair<string, string>(arg, matchGroups[arg].Value.Trim()))
                .Skip(1);

            return ComparePropertiesToArgsAndValues(template.Properties, argsAndValues);
        }

        private bool ComparePropertiesToArgsAndValues(IEnumerable<BotCommandProperty> properties, IEnumerable<KeyValuePair<string, string>> argsAndValues)
        {
            foreach (var property in properties)
            {
                var value = argsAndValues.FirstOrDefault(arg => arg.Key?.ToLowerInvariant() == property.Name.ToLowerInvariant()).Value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (!property.IsOptional)
                    {
                        throw new NotEnoughArgumentsException();
                    }
                    continue;
                }
                if (property.Type == BotCommandPropertyType.Number && !int.TryParse(value, out _))
                {
                    throw new InvalidArgumentsException();
                }
                if (property.Type == BotCommandPropertyType.Time && !this._exTime.IsMatch(value))
                {
                    throw new InvalidArgumentsException();
                }
                if (property.Type == BotCommandPropertyType.UserMention && !this._exUserMention.IsMatch(value))
                {
                    throw new InvalidArgumentsException();
                }
                if (property.Type == BotCommandPropertyType.ChannelMention && !this._exChannelMention.IsMatch(value))
                {
                    throw new InvalidArgumentsException();
                }
            }
            return true;
        }
    }
}