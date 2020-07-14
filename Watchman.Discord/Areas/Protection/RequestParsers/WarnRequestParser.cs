using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class WarnRequestParser : RequestParser
    {
        public WarnRequestParser(DiscordRequest request, UsersService usersService, Contexts contexts) : base(request, usersService, contexts)
        {
        }

        public WarnEvent GetWarnEvent()
        {
            var granterContext = _usersService.GetUserById(_contexts.Server, _contexts.User.Id);
            var userToWarnContext = GetUser();

            var grantor = new User(granterContext.Id, granterContext.Name);
            var receiver = new User(userToWarnContext.Id, userToWarnContext.Name);

            var reason = _request.Arguments.FirstOrDefault(x => x.Name == "reason" || x.Name == "r")?.Value;
            var warnTime = _request.SentAt;
            return new WarnEvent(grantor, receiver, reason ?? "No reason specified", warnTime, _contexts.Server.Id);
        }

        public ulong GetWarnsServerId()
        {
            bool allServers = _request.Arguments.Any(x => x.Name == "all");

            if (allServers)
            {
                return 0;
            }
            else
            {
                return _contexts.Server.Id;
            }
        }
    }
}
