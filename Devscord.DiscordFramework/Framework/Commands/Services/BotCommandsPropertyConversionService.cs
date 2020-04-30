using Devscord.DiscordFramework.Framework.Commands.Properties;
using System;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsPropertyConversionService
    {
        private readonly Regex exTime = new Regex(@"(?<Value>\d+)(?<Unit>(h|m|s))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex exMention = new Regex(@"<@&?\d+>", RegexOptions.Compiled);

        public object ConvertType(string value, BotCommandPropertyType type)
        {
            return type switch
            {
                BotCommandPropertyType.Time => ToTimeSpan(value),
                BotCommandPropertyType.Number => int.Parse(value),//TODO add more types
                _ => value
            };
        }

        private TimeSpan ToTimeSpan(string value)
        {
            var match = exTime.Match(value);
            var unit = match.Groups["Unit"].Value.ToLowerInvariant();
            var timeValue = short.Parse(match.Groups["Value"].Value);
            return unit switch
            {
                "h" => TimeSpan.FromHours(timeValue),
                "m" => TimeSpan.FromMinutes(timeValue),
                "s" => TimeSpan.FromSeconds(timeValue),
                _ => throw new ArgumentNullException()
            };
        }
    }
}
