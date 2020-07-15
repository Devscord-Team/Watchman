using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Linq;
using Watchman.Discord.Areas.Commons;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class MuteRequestParser : RequestParser
    {
        public MuteRequestParser(DiscordRequest request, UsersService usersService, Contexts contexts) : base(request, usersService, contexts)
        {
        }

        public MuteEvent GetMuteEvent(ulong userId, Contexts contexts, DiscordRequest request)
        {
            var reason = this._request.Arguments.FirstOrDefault(x => x.Name == "reason" || x.Name == "r")?.Value;
            var timeRange = request.GetFutureTimeRange(defaultTime: TimeSpan.FromHours(1));
            return new MuteEvent(userId, timeRange, reason, contexts.Server.Id);
        }
    }
}
