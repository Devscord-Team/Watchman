using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Administration.Models;
using Watchman.DomainModel.Messages.Queries;

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
            var readUserMessagesRequest = new ReadUserMessagesRequest(request.Arguments);
            if(string.IsNullOrWhiteSpace(readUserMessagesRequest.Mention))
            {
                throw new UserNotFoundException(string.Empty);
            }
            var selectedUser = _usersService.GetUsers(contexts.Server).FirstOrDefault(x => x.Mention == readUserMessagesRequest.Mention);
            if(selectedUser == null)
            {
                throw new UserNotFoundException(readUserMessagesRequest.Mention);
            }
            var query = new GetUserMessagesQuery(contexts.Server.Id, selectedUser.Id)
            {
                CreatedDate = readUserMessagesRequest.GetTimeRange()
            };
            var messages = _queryBus.Execute(query).Messages;
            var result = new StringBuilder().PrintManyLines($"Messages from user {selectedUser} in last {readUserMessagesRequest.MinutesSince} minutes", messages.Select(x => $"{x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")} {x.Author.ToString()}: {x.Content}").ToArray());
        }
    }
}
