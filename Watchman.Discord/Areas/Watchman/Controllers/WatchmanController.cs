using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Watchman.Discord.Areas.Watchman.BotCommands;

namespace Watchman.Discord.Areas.Watchman.Controllers
{
    public class WatchmanController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public WatchmanController(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public Task InviteMe(InviteMeCommand command, Contexts contexts)
        {
            const string urlAddress = "https://discordapp.com/api/oauth2/authorize?client_id=636274997786312723&permissions=2147483127&scope=bot";
            var messagesService = this._messagesServiceFactory.Create(contexts);
            return messagesService.SendResponse(x => x.InviteMe(contexts, urlAddress));
        }
    }
}
