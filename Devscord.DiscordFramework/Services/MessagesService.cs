using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class MessagesService : IService
    {
        private readonly ResponsesService responsesService;

        public ulong ChannelId { get; set; }

        public MessagesService()
        {
        }

        public MessagesService(ResponsesService responsesService)
        {
            this.responsesService = responsesService;
        }

        public Task SendMessage(string message)
        {
            var channel = (ISocketMessageChannel)Server.GetChannel(ChannelId);
            return channel.SendMessageAsync(message);
        }

        public Task SendResponse(Func<ResponsesService, string> response, Contexts contexts)
        {
            responsesService.RefreshResponses(contexts);
            var message = response.Invoke(this.responsesService);
            return this.SendMessage(message);
        }

        public Task SendFile(string filePath)
        {
            var channel = (ISocketMessageChannel)Server.GetChannel(ChannelId);
            return channel.SendFileAsync(filePath);
        }
    }
}
