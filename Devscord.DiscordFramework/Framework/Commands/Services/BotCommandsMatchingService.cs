using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsMatchingService
    {
        private readonly Regex _exTime = new Regex(@"\d+(ms|d|h|m|s)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex _exUserMention = new Regex(@"\<@!?\d+\>", RegexOptions.Compiled);
        private readonly Regex _exChannelMention = new Regex(@"\<#\d+\>", RegexOptions.Compiled);

        public bool IsDefaultCommand(BotCommandTemplate template, IEnumerable<DiscordRequestArgument> arguments, bool isCommandMatchedWithCustom)
        {
            var requiredProperties = template.Properties.Where(x => !x.IsOptional);
            if (requiredProperties.Any(property => arguments.All(arg => property.Name.ToLowerInvariant() != arg.Name?.ToLowerInvariant())))
            {
                return false;
            }
            if (!isCommandMatchedWithCustom)
            {
                return true;
            }
            var argsWithoutListSubarguments = arguments
                .Select(x => x.Name?.ToLowerInvariant())
                .ToList();
            var namesOfLists = template.Properties
                .Where(property => property.Type == BotCommandPropertyType.List)
                .Select(x => x.Name.ToLowerInvariant())
                .ToList();
            for (int i = 0; i < argsWithoutListSubarguments.Count; i++)
            {
                if (!namesOfLists.Contains(argsWithoutListSubarguments[i]))
                {
                    continue;
                }
                while (i + 1 < argsWithoutListSubarguments.Count && argsWithoutListSubarguments[i + 1] == null)
                {
                    argsWithoutListSubarguments.RemoveAt(i + 1);
                }
            }
            return argsWithoutListSubarguments.All(arg => template.Properties.Any(property => arg == property.Name.ToLowerInvariant()));
        }

        public bool AreDefaultCommandArgumentsCorrect(BotCommandTemplate template, IEnumerable<DiscordRequestArgument> arguments)
        {  
            var argsAndValues = arguments
                .Select(arg => new KeyValuePair<string, string>(arg.Name, arg.Value))
                .ToList();

            this.CheckQuotationMarksInListsAndTexts(template.Properties, argsAndValues);
            return this.ComparePropertiesToArgsAndValues(template.Properties, argsAndValues);
        }

        public bool AreCustomCommandArgumentsCorrect(BotCommandTemplate template, Regex customTemplate, string input)
        {
            var matchGroups = customTemplate.Match(input).Groups;
            var requiredProperties = template.Properties.Where(x => !x.IsOptional);
            if (requiredProperties.Any(x => !matchGroups.ContainsKey(x.Name)))
            {
                Log.Warning("Custom template {customTemplate} is not valid for {commandName}", customTemplate, template.CommandName);
                throw new InvalidArgumentsException();
            }
            var argsAndValues = matchGroups.Keys
                .Where(arg => !arg.All(char.IsDigit))
                .Select(arg => new KeyValuePair<string, string>(arg, matchGroups[arg].Value.Trim()))
                .ToList();

            this.CheckQuotationMarksInListsAndTexts(template.Properties, argsAndValues, isCustomComand: true);
            return this.ComparePropertiesToArgsAndValues(template.Properties, argsAndValues);
        }

        private void CheckQuotationMarksInListsAndTexts(IEnumerable<BotCommandProperty> properties, IEnumerable<KeyValuePair<string, string>> argsAndValues, bool isCustomComand = false)
        {
            foreach (var property in properties)
            {
                if (property.Type != BotCommandPropertyType.Text && property.Type != BotCommandPropertyType.List)
                {
                    continue;
                }
                var argumentAndValue = argsAndValues.FirstOrDefault(arg => arg.Key?.ToLowerInvariant() == property.Name.ToLowerInvariant());
                if (string.IsNullOrWhiteSpace(argumentAndValue.Value))
                {
                    continue;
                }
                if (isCustomComand && argumentAndValue.Value.Count(x => x == '\"') % 2 != 0)
                {
                    throw new InvalidArgumentsException();
                }
                if (!isCustomComand)
                {
                    var countOfArgsBeforeCurrentOne = argsAndValues.ToList().IndexOf(argumentAndValue);
                    var hasRedundantQuotationMarks = argsAndValues
                        .Skip(countOfArgsBeforeCurrentOne)
                        .TakeWhile(x => x.Key?.ToLowerInvariant() == property.Name.ToLowerInvariant() || x.Key == null)
                        .Any(x => x.Value.Contains('\"'));

                    if (hasRedundantQuotationMarks)
                    {
                        throw new InvalidArgumentsException();
                    }
                }
            }
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
                if (property.Type == BotCommandPropertyType.Number && !value.All(char.IsDigit))
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
