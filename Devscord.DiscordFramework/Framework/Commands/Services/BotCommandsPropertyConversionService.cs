using Devscord.DiscordFramework.Framework.Commands.Properties;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsPropertyConversionService
    {
        private readonly Regex _exTime = new Regex(@"(?<Value>\d+)(?<Unit>(ms|d|h|m|s))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex _exMention = new Regex(@"\d+", RegexOptions.Compiled);

        public object ConvertType(string value, BotCommandPropertyType type)
        {
            return type switch
            {
                BotCommandPropertyType.Time => this.ToTimeSpan(value),
                BotCommandPropertyType.Number => int.Parse(value),
                BotCommandPropertyType.Bool => bool.Parse(value),
                BotCommandPropertyType.UserMention => ulong.Parse(_exMention.Match(value).Value),
                BotCommandPropertyType.ChannelMention => ulong.Parse(_exMention.Match(value).Value),
                _ => value
            };
        }

        private TimeSpan ToTimeSpan(string value)
        {
            var match = this._exTime.Match(value);
            var unit = match.Groups["Unit"].Value.ToLowerInvariant();
            var timeValue = uint.Parse(match.Groups["Value"].Value);
            return unit switch
            {
                "d" => TimeSpan.FromDays(timeValue),
                "h" => TimeSpan.FromHours(timeValue),
                "m" => TimeSpan.FromMinutes(timeValue),
                "s" => TimeSpan.FromSeconds(timeValue),
                "ms" => TimeSpan.FromMilliseconds(timeValue),
                _ => throw new ArgumentNullException()
            };
        }
    }
}
