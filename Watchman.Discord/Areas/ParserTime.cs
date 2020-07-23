using Devscord.DiscordFramework.Commons.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Watchman.Common.Models;

namespace Watchman.Discord.Areas
{
    public class ParserTime
    {
        public TimeRange GetFutureTimeRange(string timeAsString, TimeSpan defaultTime)
        {
            return new TimeRange(
                start: DateTime.UtcNow,
                end: DateTime.UtcNow + ParseToTimeSpan(timeAsString, defaultTime));
        }

        public TimeRange GetPastTimeRange(string timeAsString, TimeSpan defaultTime)
        {
            return new TimeRange(
                start: DateTime.UtcNow - ParseToTimeSpan(timeAsString, defaultTime),
                end: DateTime.UtcNow);
        }

        private TimeSpan ParseToTimeSpan(string timeAsString, TimeSpan defaultTime)
        {
            var lastChar = timeAsString?[^1];
            timeAsString = timeAsString?[..^1];
            timeAsString = timeAsString?.Replace(',', '.');
            double.TryParse(timeAsString, NumberStyles.Any, CultureInfo.InvariantCulture, out var timeAsNumber);

            if (timeAsNumber <= 0)
            {
                throw new InvalidArgumentsException();
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
