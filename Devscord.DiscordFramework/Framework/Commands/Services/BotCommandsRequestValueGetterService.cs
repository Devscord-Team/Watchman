using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Properties;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsRequestValueGetterService
    {
        public object GetValueByName(string key, bool isList, DiscordRequest request, BotCommandTemplate template)
        {
            var result = request.Arguments.FirstOrDefault(a => (a.Name?.ToLowerInvariant() ?? string.Empty) == key.ToLowerInvariant());
            if (result == null)
            {
                return null;
            }
            var argType = template.Properties.FirstOrDefault(x => x.Name.ToLowerInvariant() == key.ToLowerInvariant())
                ?.Type;
            if (argType.HasValue && argType.Value == BotCommandPropertyType.Bool)
            {
                return result.Value ?? bool.TrueString;
            }
            if (!isList)
            {
                return result.Value ?? string.Empty;
            }
            var argumentsList = request.Arguments.ToList();
            var indexOf = argumentsList.IndexOf(result);
            var nextResults = argumentsList.Skip(indexOf + 1).TakeWhile(x => x.Name == null);
            var list = new List<string> { result.Value };
            list.AddRange(nextResults.Select(x => x.Value));
            return list;
        }

        public object GetValueByNameFromCustomCommand(string key, bool isList, BotCommandTemplate template, Match match)
        {
            if (!match.Groups.ContainsKey(key))
            {
                return null;
            }
            var lowerCaseKey = key.ToLowerInvariant();
            var value = match.Groups[key].Value.Trim();
            var argType = template.Properties.FirstOrDefault(x => x.Name.ToLowerInvariant() == lowerCaseKey)?.Type;
            if (argType.HasValue && argType.Value == BotCommandPropertyType.Bool && !string.IsNullOrWhiteSpace(value))
            {
                return bool.TrueString;
            }
            if (!isList)
            {
                return value;
            }
            if (!value.Contains('\"') || value.Count(x => x == '\"') % 2 != 0)
            {
                return value.Split(' ').ToList();
            }
            var results = value.Split('"')
                .Where(x => !string.IsNullOrWhiteSpace(x) && !x.StartsWith(' ') && !x.EndsWith(' '))
                .ToList();
            foreach (var toRemove in results)
            {
                value = value.Replace($"\"{toRemove}\"", string.Empty);
            }
            var otherResults = value.Split(' ').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim());
            results.AddRange(otherResults);
            return results;
        }
    }
}
