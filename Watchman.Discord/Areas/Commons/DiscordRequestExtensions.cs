using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Watchman.Common.Models;

namespace Watchman.Discord.Areas.Commons
{
    public static class DiscordRequestExtensions
    {
        public static string GetMention(this DiscordRequest discordRequest)
        {
            var mention = discordRequest.Arguments.FirstOrDefault(x => (x.Value?.StartsWith('<') ?? false) && (x.Value?.EndsWith('>') ?? false))?.Value;
            if (mention == null)
            {
                throw new UserDidntMentionAnyUserException();
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

        public static bool HasDuplicates(this IEnumerable<DiscordRequestArgument> requestArguments)
        {
            return requestArguments.Count() != requestArguments.Select(x => x.Value).Distinct().Count();
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

            var parsedTimeSpan = lastChar switch
            {
                's' => TimeSpan.FromSeconds(timeAsNumber),
                'm' => TimeSpan.FromMinutes(timeAsNumber),
                'h' => TimeSpan.FromHours(timeAsNumber),
                _ => defaultTime,
            };

            if (parsedTimeSpan.TotalMilliseconds >= int.MaxValue)
            {
                throw new TimeIsTooBigException();
            }
            return parsedTimeSpan;
        }
    }
}
