using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.Discord.Areas.Users.BotCommands.Warns;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class WarnsController : IController
    {
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IUsersService _usersService;
        private readonly IWarnsService _warnService;

        public WarnsController(IMessagesServiceFactory messagesServiceFactory, IUsersService usersService, IWarnsService warnService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._usersService = usersService;
            this._warnService = warnService;
        }

        [AdminCommand]
        public async Task AddWarn(AddWarnCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var mentionedUser = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            await this._warnService.AddWarnToUser(command, contexts, mentionedUser);
            await messageService.SendResponse(x => x.UserHasBeenWarned(contexts.User.Name, mentionedUser.Name, command.Reason));
        }

        public async Task GetWarns(WarnsCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var mentionedUser = command.User == 0 ? contexts.User : await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            if (mentionedUser == null)
            {
                throw new UserNotFoundException(command.User.GetUserMention());
            }
            var warns = this._warnService.GetWarns(mentionedUser, contexts.Server.Id);
            await messageService.SendEmbedMessage("Ostrzeżenia", string.Empty, warns);
        }
    }
}
