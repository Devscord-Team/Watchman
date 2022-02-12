using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Commands;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Commands.Properties;

namespace Devscord.DiscordFramework.Commands.Services
{
    public class BotCommandsRequestValueGetterService
    {
        public object GetValueByName(string key, bool isList, DiscordRequest request, BotCommandTemplate template)
        {
            var argType = template.Properties.First(x => x.Name.ToLowerInvariant() == key.ToLowerInvariant()).Type;
            var result = request.Arguments.FirstOrDefault(a => a.Name?.ToLowerInvariant() == key.ToLowerInvariant());
            if (argType == BotCommandPropertyType.Bool)
            {
                return result == null ? bool.FalseString : bool.TrueString;
            }
            if (result == null)
            {
                return null;
            }
            if (argType == BotCommandPropertyType.Text)
            {
                return result.Value.Trim('\"');
            }
            if (!isList)
            {
                return result.Value;
            }
            var indexOf = request.Arguments.ToList().IndexOf(result);
            var list = request.Arguments
                .Skip(indexOf)
                .TakeWhile(x => x.Name == result.Name || x.Name == null)
                .Select(x => x.Value)
                .ToList();

            return list;
        }

        public object GetValueByNameFromCustomCommand(string key, bool isList, BotCommandTemplate template, Match match)
        {
            var value = match.Groups[key].Value.Trim();
            var argType = template.Properties.First(x => x.Name.ToLowerInvariant() == key.ToLowerInvariant()).Type;
            if (argType == BotCommandPropertyType.Bool)
            {
                return string.IsNullOrWhiteSpace(value) ? bool.FalseString : bool.TrueString;
            }
            if (argType == BotCommandPropertyType.SingleWord)
            {
                return value.Split().First().Trim('\"'); 
            }
            if (argType == BotCommandPropertyType.Text)
            {
                return value.Trim('\"');
            }
            if (!isList)
            {
                return value;
            }

            if (!value.Contains('\"'))
            {
                return value.Split().Where(x => !string.IsNullOrEmpty(x))
                    .ToList();
            }
            var splittedResults = value.Split('"').Where(x => !string.IsNullOrWhiteSpace(x));
            var results = new List<string>();
            foreach (var toRemove in splittedResults)
            {
                if (value.Contains($"\"{toRemove}\""))
                {
                    // here we're adding text with quotation marks to the results, but texts without quotation marks are added later
                    results.Add(toRemove);
                }
                value = value.Replace($"\"{toRemove}\"", string.Empty);
            }
            var otherResultsWithoutQuote = value.Split().Where(x => !string.IsNullOrWhiteSpace(x));
            results.AddRange(otherResultsWithoutQuote);
            return results;
        }
    }
}
