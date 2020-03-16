using System;
using System.Globalization;
using System.Linq;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Common.Models;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MuteRequestParser
    {
        private readonly DiscordRequest _request;
        private readonly UsersService _usersService;
        private readonly Contexts _contexts;

        public MuteRequestParser(DiscordRequest request, UsersService usersService, Contexts contexts)
        {
            _request = request;
            _usersService = usersService;
            _contexts = contexts;
        }

        public UserContext GetUser()
        {
            var mention = GetMention();

            var userToMute = _usersService.GetUsers(_contexts.Server)
                .FirstOrDefault(x => x.Mention == mention);

            if (userToMute == null)
            {
                throw new UserNotFoundException(mention);
            }
            return userToMute;
        }

        public MuteEvent GetMuteEvent(ulong userId, Contexts contexts, DateTime startTime)
        {
            var reason = _request.Arguments.FirstOrDefault(x => x.Name == "reason")?.Value;
            var forTime = _request.Arguments.FirstOrDefault(x => x.Name == "time")?.Value;

            var timeRange = TimeRange.Create(startTime, startTime + ParseToTimeSpan(forTime));
            return new MuteEvent(userId, timeRange, reason, contexts.Server.Id);
        }

        private string GetMention()
        {
            var mention = _request.Arguments.FirstOrDefault()?.Value;

            if (string.IsNullOrWhiteSpace(mention))
            {
                throw new UserDidntMentionedAnyUserToMuteException();
            }
            return mention;
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

            if (asNumber <= 0)
            {
                throw new TimeCannotBeNegativeException();
            }

            if (asNumber >= int.MaxValue)
            {
                throw new TimeIsTooBigException();
            }

            return lastChar switch
            {
                'm' => TimeSpan.FromMinutes(asNumber),
                'h' => TimeSpan.FromHours(asNumber),
                _ => defaultTime,
            };
        }
    }
}
