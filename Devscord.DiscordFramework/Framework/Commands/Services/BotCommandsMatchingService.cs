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
        private readonly UniversalNumberParser _numberParser;

        public BotCommandsMatchingService(UniversalNumberParser numberParser)
        {
            this._numberParser = numberParser;
        }

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
            return this.AreAllGivenArgsForCommandKnown(template.Properties, arguments);
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

            this.CheckQuotationMarksInListsAndTexts(template.Properties, argsAndValues, isCommandMatchedWithCustom: true);
            return this.ComparePropertiesToArgsAndValues(template.Properties, argsAndValues, isCommandMatchedWithCustom: true);
        }

        private bool AreAllGivenArgsForCommandKnown(IEnumerable<BotCommandProperty> properties, IEnumerable<DiscordRequestArgument> arguments)
        {
            var argumentsNames = arguments
                .Select(x => x.Name?.ToLowerInvariant())
                .ToList();
            var lists = properties
                .Where(property => property.GeneralType == BotCommandPropertyType.List)
                .Select(x => x.Name.ToLowerInvariant())
                .ToList();
            var argsWithoutListSubarguments = this.GetArgsWithoutListSubarguments(argumentsNames, lists);
            return argsWithoutListSubarguments.All(arg => properties.Any(property => arg == property.Name.ToLowerInvariant()));
        }

        private List<string> GetArgsWithoutListSubarguments(List<string> argumentsNames, List<string> listsNames)
        {
            for (int i = 0; i < argumentsNames.Count; i++)
            {
                if (!listsNames.Contains(argumentsNames[i]))
                {
                    continue;
                }
                while (i + 1 < argumentsNames.Count && argumentsNames[i + 1] == null)
                {
                    argumentsNames.RemoveAt(i + 1);
                }
            }
            return argumentsNames;
        }

        private void CheckQuotationMarksInListsAndTexts(IEnumerable<BotCommandProperty> properties, IEnumerable<KeyValuePair<string, string>> argsAndValues, bool isCommandMatchedWithCustom = false)
        {
            foreach (var property in properties)
            {
                if (property.GeneralType != BotCommandPropertyType.Text && property.GeneralType != BotCommandPropertyType.List)
                {
                    continue;
                }
                var argumentAndValue = argsAndValues.FirstOrDefault(arg => arg.Key?.ToLowerInvariant() == property.Name.ToLowerInvariant());
                if (string.IsNullOrWhiteSpace(argumentAndValue.Value))
                {
                    continue;
                }
                if (isCommandMatchedWithCustom && argumentAndValue.Value.Count(x => x == '\"') % 2 != 0)
                {
                    throw new InvalidArgumentsException();
                }
                if (!isCommandMatchedWithCustom)
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

        private bool ComparePropertiesToArgsAndValues(IEnumerable<BotCommandProperty> properties, IEnumerable<KeyValuePair<string, string>> argsAndValues, bool isCommandMatchedWithCustom = false)
        {
            foreach (var property in properties)
            {
                var value = argsAndValues.FirstOrDefault(arg => arg.Key?.ToLowerInvariant() == property.Name.ToLowerInvariant()).Value;
                if (string.IsNullOrWhiteSpace(value))
                {
                    if (!property.IsOptional)
                    {
                        throw new InvalidArgumentsException();
                    }
                    var HasArgBeenGiven = argsAndValues.FirstOrDefault(arg => arg.Key?.ToLowerInvariant() == property.Name.ToLowerInvariant()).Key != null;
                    if (!isCommandMatchedWithCustom && property.GeneralType != BotCommandPropertyType.Bool && HasArgBeenGiven)
                    {
                        throw new InvalidArgumentsException();
                    }
                    continue;
                }
                switch (property.GeneralType)
                {
                    case BotCommandPropertyType.Number when !IsNumberValid(value, property.ActualType):
                    case BotCommandPropertyType.Time when !this._exTime.IsMatch(value):
                    case BotCommandPropertyType.UserMention when !this._exUserMention.IsMatch(value):
                    case BotCommandPropertyType.ChannelMention when !this._exChannelMention.IsMatch(value):
                        throw new InvalidArgumentsException();
                }
            }
            return true;
        }

        private bool IsNumberValid(string value, Type type)
        {
            try
            {
                this._numberParser.Parse(value, type);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
