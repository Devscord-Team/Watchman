using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
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
using Watchman.Discord.Areas.Users.Services;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Users;
using Watchman.DomainModel.Warns.Commands;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class WarnsController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersService _usersService;
        private readonly IWarnsService _warnService;

        public WarnsController(MessagesServiceFactory messagesServiceFactory, UsersService usersService, IWarnsService warnService)
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
            await this._warnService.AddWarnToUser(contexts.User.Id, mentionedUser.Id, command.Reason, contexts.Server.Id);
            await messageService.SendResponse(x => x.UserHasBeenWarned(contexts.User.Name, mentionedUser.Name, command.Reason));
        }

        [AdminCommand]
        public async Task ResetWarns(ResetWarnsCommand command, Contexts contexts)
        {
            await _warnService.RemoveUserWarns(command.User, contexts.Server.Id);
            var messageService = this._messagesServiceFactory.Create(contexts);
            await messageService.SendResponse(x => x.AllWarnsRemovedFromUser(contexts.User.Id, command.User));
        }

        public async Task Warns(WarnsCommand command, Contexts contexts)
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
