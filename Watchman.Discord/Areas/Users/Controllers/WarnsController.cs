using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
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
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class WarnsController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly WarnService _warnService;

        public WarnsController(MessagesServiceFactory messagesServiceFactory, UsersService usersService, DirectMessagesService directMessagesService, WarnService warnService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._warnService = warnService;
        }

        [AdminCommand]
        public async Task AddWarn(AddWarnCommand command, Contexts contexts)
        {
            var msgService = _messagesServiceFactory.Create(contexts);
            var mentionedUser = _usersService.GetUserByMention(contexts.Server, command.User.ToString());
            await _warnService.AddWarnToUser(command, contexts, mentionedUser);
            await msgService.SendResponse(x => x.UserHasBeenWarned(contexts.User.Name, mentionedUser.Name, command.Reason));
        }

        public async Task Warns(WarnsCommand command, Contexts contexts)
        {
            var mentionedUser = command.User == 0 ? contexts.User : _usersService.GetUserById(contexts.Server, command.User);
            var serverId = command.All ? 0 : contexts.Server.Id;
            var msgService = _messagesServiceFactory.Create(contexts);

            if (!command.All)
            {
                if (mentionedUser == null)
                {
                    await msgService.SendResponse(x => x.UserNotFound(command.User.ToString()));
                }
                else
                {
                    var warnsStr = await _warnService.GetWarnsToString(serverId, mentionedUser.Id);
                    await msgService.SendResponse(x => x.GetUserWarns(mentionedUser.Name, warnsStr));
                }
                return;
            }

            if (contexts.User.IsAdmin)
            {
                if (mentionedUser == null)
                {
                    await _directMessagesService.TrySendMessage(contexts.User.Id, x => x.UserNotFound(command.User.ToString()), contexts);
                }
                else
                {
                    var warnsStr = await _warnService.GetWarnsToString(serverId, mentionedUser.Id);
                    await _directMessagesService.TrySendMessage(contexts.User.Id, x => x.GetUserWarns(mentionedUser.Name, warnsStr), contexts);
                }                
                return;
            }

            throw new NotAdminPermissionsException();
        }
    }
}
