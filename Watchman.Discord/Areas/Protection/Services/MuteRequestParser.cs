using System;
using System.Linq;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Discord.Areas.Commons;
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
            var mention = _request.GetMention();
            var userToMute = _usersService.GetUserByMention(_contexts.Server, mention);

            if (userToMute == null)
            {
                throw new UserNotFoundException(mention);
            }
            return userToMute;
        }

        public MuteEvent GetMuteEvent(ulong userId, Contexts contexts, DiscordRequest request)
        {
            var reason = _request.Arguments.FirstOrDefault(x => x.Name == "reason" || x.Name == "r")?.Value;
            var timeRange = request.GetFutureTimeRange(defaultTime: TimeSpan.FromHours(1));
            return new MuteEvent(userId, timeRange, reason, contexts.Server.Id);
        }
    }
}
