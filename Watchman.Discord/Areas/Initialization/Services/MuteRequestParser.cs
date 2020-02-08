using System;
using System.Globalization;
using System.Linq;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Common.Exceptions;
using Watchman.Common.Models;
using Watchman.DomainModel.Mute;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class MuteRequestParser
    {
        private readonly DiscordRequest _request;

        public MuteRequestParser(DiscordRequest request)
        {
            _request = request;
        }

        public string GetMention()
        {
            var mention = _request.Arguments.FirstOrDefault()?.Values.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(mention))
            {
                throw new UserDidntMentionedAnyUserToMuteException();
            }
            return mention;
        }

        public MuteEvent GetMuteEvent(ulong userId, Contexts contexts)
        {
            var reason = _request.Arguments.FirstOrDefault(x => x.Name == "reason")?.Values.FirstOrDefault();
            var forTime = _request.Arguments.FirstOrDefault(x => x.Name == "time")?.Values.FirstOrDefault();

            var timeRange = new TimeRange()
            {
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow + ParseToTimeSpan(forTime)
            };

            return new MuteEvent(userId, timeRange, reason, contexts.Server.Id);
        }

        private static TimeSpan ParseToTimeSpan(string time)
        {
            var defaultTime = TimeSpan.FromHours(1);

            if (string.IsNullOrWhiteSpace(time))
                return defaultTime;

            var lastChar = time[^1];
            time = time[..^1];
            time = time.Replace(',', '.');
            double.TryParse(time, NumberStyles.Any, CultureInfo.InvariantCulture, out var asNumber);

            return lastChar switch
            {
                'm' => TimeSpan.FromMinutes(asNumber),
                'h' => TimeSpan.FromHours(asNumber),
                _ => defaultTime,
            };
        }
    }
}
