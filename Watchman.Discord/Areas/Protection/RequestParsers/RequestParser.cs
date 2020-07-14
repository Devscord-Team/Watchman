using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Discord.Areas.Commons;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class RequestParser
    {
        protected readonly DiscordRequest _request;
        protected readonly UsersService _usersService;
        protected readonly Contexts _contexts;

        public RequestParser(DiscordRequest request, UsersService usersService, Contexts contexts)
        {
            this._request = request;
            this._usersService = usersService;
            this._contexts = contexts;
        }

        public UserContext GetUser()
        {
            var mention = this._request.GetMention();
            var user = this._usersService.GetUserByMention(this._contexts.Server, mention);

            if (user == null)
            {
                throw new UserNotFoundException(mention);
            }
            return user;
        }
    }
}
