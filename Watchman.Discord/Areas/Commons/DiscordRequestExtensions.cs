using System;
using System.Globalization;
using System.Linq;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Watchman.Common.Models;

namespace Watchman.Discord.Areas.Commons
{
    public static class DiscordRequestExtensions
    {
        public static string GetMention(this DiscordRequest discordRequest)
        {
            var mention = discordRequest.Arguments.FirstOrDefault(x => x.Value.StartsWith('<') && x.Value.EndsWith('>'))?.Value;
            if (mention == null)
            {
                throw new UserDidntMentionAnyUser();
            }
            return mention;
        }

        public static TimeRange GetFutureTimeRange(this DiscordRequest discordRequest, TimeSpan defaultTime)
        {
            return new TimeRange(
                start: discordRequest.SentAt,
                end: discordRequest.SentAt + ParseToTimeSpan(discordRequest, defaultTime));
        }

        public static TimeRange GetPastTimeRange(this DiscordRequest discordRequest, TimeSpan defaultTime)
        {
            return new TimeRange(
                start: discordRequest.SentAt - ParseToTimeSpan(discordRequest, defaultTime), 
                end: discordRequest.SentAt);
        }

        private static TimeSpan ParseToTimeSpan(DiscordRequest discordRequest, TimeSpan defaultTime)
        {
            var timeAsString = discordRequest.Arguments.FirstOrDefault(x => x.Name == "t" || x.Name == "time")?.Value;
            if (string.IsNullOrWhiteSpace(timeAsString))
            {
                return defaultTime;
            }

            var lastChar = timeAsString[^1];
            timeAsString = timeAsString[..^1];
            timeAsString = timeAsString.Replace(',', '.');
            double.TryParse(timeAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out var timeAsNumber);

            if (timeAsNumber <= 0)
            {
                throw new TimeCannotBeNegativeException();
            }

            // huge value will be too big for parsing to DateTime, so I use ushort (instead of int) to be sure that the value isn't too big
            if (timeAsNumber >= ushort.MaxValue) 
            {
                throw new TimeIsTooBigException();
            }

            var parsedTimeSpan = lastChar switch
            {
                's' => TimeSpan.FromSeconds(timeAsNumber),
                'm' => TimeSpan.FromMinutes(timeAsNumber),
                'h' => TimeSpan.FromHours(timeAsNumber),
                _ => defaultTime,
            };
            return parsedTimeSpan;
        }
    }
}
