using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Properties;
using Devscord.DiscordFramework.Integration;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsPropertyConversionService
    {
        private readonly Regex _exTime = new Regex(@"(?<Value>\d+)(?<Unit>(ms|d|h|m|s))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex _exMention = new Regex(@"\d+", RegexOptions.Compiled);

        public object ConvertType(string value, BotCommandProperty botCommandProperty)
        {
            return botCommandProperty.GeneralType switch
            {
                BotCommandPropertyType.Time => this.ToTimeSpan(value),
                BotCommandPropertyType.Number => Convert.ChangeType(value, Nullable.GetUnderlyingType(botCommandProperty.ActualType) ?? botCommandProperty.ActualType),
                BotCommandPropertyType.Bool => bool.Parse(value),
                BotCommandPropertyType.UserMention => ulong.Parse(_exMention.Match(value).Value),
                BotCommandPropertyType.ChannelMention => ulong.Parse(_exMention.Match(value).Value),
                _ => value
            };
        }

        private TimeSpan ToTimeSpan(string value)
        {
            var matchGroups = this._exTime.Match(value).Groups;
            var unit = matchGroups["Unit"].Value.ToLowerInvariant();
            var timeValue = uint.Parse(matchGroups["Value"].Value);
            return unit switch
            {
                "d" => TimeSpan.FromDays(timeValue),
                "h" => TimeSpan.FromHours(timeValue),
                "m" => TimeSpan.FromMinutes(timeValue),
                "s" => TimeSpan.FromSeconds(timeValue),
                _ => TimeSpan.FromMilliseconds(timeValue),
            };
        }
    }
}
