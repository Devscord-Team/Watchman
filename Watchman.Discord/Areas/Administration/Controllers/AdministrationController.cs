using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;

        public AdministrationController(IQueryBus queryBus, UsersService usersService)
        {
            this._queryBus = queryBus;
            this._usersService = usersService;
        }

        [AdminCommand]
        [DiscordCommand("messages")]
        public void ReadUserMessages(DiscordRequest request, Contexts contexts)
        {
            var mention = request.Arguments.FirstOrDefault(x => x.Value.StartsWith('<') && x.Value.StartsWith('>'))?.Value;
            if(string.IsNullOrWhiteSpace(mention))
            {
                throw new UserNotFoundException(string.Empty);
            }
            var selectedUser = _usersService.GetUsers(contexts.Server).FirstOrDefault(x => x.Mention == mention);
        }
    }
}
