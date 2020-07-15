using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Areas.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class WarnUserController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly WarnService _warnService;

        public WarnUserController(MessagesServiceFactory messagesServiceFactory, UsersService usersService, DirectMessagesService directMessagesService, WarnService warnService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._warnService = warnService;
        }

        [DiscordCommand("addwarn")]
        [AdminCommand]
        public async Task WarnUser(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new WarnRequestParser(request, this._usersService, contexts);
            var warnEvent = requestParser.GetWarnEvent();
            var msgService = _messagesServiceFactory.Create(contexts);

            if (string.IsNullOrEmpty(warnEvent.Reason) || string.IsNullOrWhiteSpace(warnEvent.Reason))
            {
                await msgService.SendMessage("You have to specify warn reason -reason -r \"reason\"", MessageType.Json);
            }
            else
            {
                await _warnService.WarnUser(warnEvent);
                await msgService.SendMessage("User " + warnEvent.Receiver.Name + " has been warned by " + warnEvent.Grantor.Name + " for: " + warnEvent.Reason, MessageType.Json);
            }
        }

        [DiscordCommand("warns")]
        public async Task GetWarns(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new WarnRequestParser(request, this._usersService, contexts);
            var serverId = requestParser.GetServerIdForWarns();
            UserContext user = null;

            try
            {
                user = requestParser.GetUser();
            }
            catch (Exception e) 
            {
                user = contexts.User;
            }

            var warnsStr = await _warnService.GetWarnsToString(serverId, user.Id);

            if (serverId != 0)
            {
                var msgService = _messagesServiceFactory.Create(contexts);
                await msgService.SendMessage(warnsStr);
            }
            else if (contexts.User.IsAdmin)
            {
                await _directMessagesService.TrySendMessage(contexts.User.Id, warnsStr);
            } 
            else
            {
                throw new NotAdminPermissionsException();
            }
        }
    }
}
