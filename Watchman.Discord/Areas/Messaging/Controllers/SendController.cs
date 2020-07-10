using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Help.BotCommands;
using Watchman.Discord.Areas.Help.Services;
using Watchman.Discord.Areas.Messaging.BotCommands;
using Watchman.Discord.Areas.Messaging.Services;
using Watchman.DomainModel.Help.Queries;

namespace Watchman.Discord.Areas.Messaging.Controllers
{
    public class SendController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly ChannelsService _channelsService;

        public SendController(MessagesServiceFactory messagesServiceFactory,ChannelsService channelsService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._channelsService = channelsService;
        }
        [DiscordCommand("send")]
        public async Task SendCommand(DiscordRequest request, Contexts contexts)
        {
            var requestParser = new SendRequestParser(request, contexts, _channelsService);
            var channelToSendMessageTo = requestParser.GetChannel();
            
            string messageToSend = requestParser.ParseMessage();
            contexts.SetContext(channelToSendMessageTo);
           
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendMessage(messageToSend);
        }
    }
}
