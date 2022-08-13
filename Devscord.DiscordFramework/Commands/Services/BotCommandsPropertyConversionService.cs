using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commands.Properties;
using Devscord.DiscordFramework.Integration;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Commands.Services
{
    public interface IBotCommandsPropertyConversionService
    {
        object ConvertType(string value, BotCommandPropertyType commandType, Type propertyType);
    }

    public class BotCommandsPropertyConversionService : IBotCommandsPropertyConversionService
    {
        private readonly Regex _exTime = new Regex(@"(?<Value>\d+)(?<Unit>(ms|d|h|m|s))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private readonly Regex _exMention = new Regex(@"\d+", RegexOptions.Compiled);

        public object ConvertType(string value, BotCommandPropertyType commandType, Type propertyType)
        {
            object convertedValue = commandType switch
            {
                BotCommandPropertyType.Time => this.ToTimeSpan(value),
                BotCommandPropertyType.Number => int.Parse(value),
                BotCommandPropertyType.Bool => bool.Parse(value),
                BotCommandPropertyType.UserMention => ulong.Parse(_exMention.Match(value).Value),
                BotCommandPropertyType.ChannelMention => ulong.Parse(_exMention.Match(value).Value),
                _ => value
            };

            var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;
            var result = Convert.ChangeType(convertedValue, underlyingType);
            return result;
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
