using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public enum MessageType
    {
        NormalText,
        Json
    }

    public class MessagesService : IService
    {
        public ulong ChannelId { get; set; }

        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _splittingService;

        public MessagesService()
        {
        }

        public MessagesService(ResponsesService responsesService, MessageSplittingService splittingService)
        {
            this._responsesService = responsesService;
            _splittingService = splittingService;
        }

        public Task SendMessage(string message, MessageType messageType = MessageType.NormalText)
        {
            var channel = (ISocketMessageChannel)Server.GetChannel(ChannelId);

            foreach (var mess in _splittingService.SplitMessage(message, messageType))
            {
                channel.SendMessageAsync(mess);
            }

            return Task.CompletedTask;
        }

        public Task SendResponse(Func<ResponsesService, string> response, Contexts contexts)
        {
            _responsesService.RefreshResponses(contexts);
            var message = response.Invoke(this._responsesService);
            return this.SendMessage(message);
        }

        public Task SendFile(string filePath)
        {
            var channel = (ISocketMessageChannel)Server.GetChannel(ChannelId);
            return channel.SendFileAsync(filePath);
        }
    }
}
