using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.BotCommands;
using Watchman.Discord.Areas.Help.Services;
using Watchman.Discord.Areas.Messaging.BotCommands;
using Watchman.DomainModel.Help.Queries;

namespace Watchman.Discord.Areas.Messaging.Controllers
{
    public class SendController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        
        public SendController(MessagesServiceFactory messagesServiceFactory)
        {
            this._messagesServiceFactory = messagesServiceFactory;
        }
        public async Task Send(SendCommand sendCommand, Contexts contexts)
        {
            var messagesService = this._messagesServiceFactory.Create(contexts);
            messagesService.ChannelId = sendCommand.Channel;
            await messagesService.SendMessage(sendCommand.Message);
        }
    }
}
