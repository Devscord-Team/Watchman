using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class MessagesService
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }

        private readonly ResponsesService _responsesService;
        private readonly MessageSplittingService _splittingService;

        public MessagesService(ResponsesService responsesService, MessageSplittingService splittingService)
        {
            this._responsesService = responsesService;
            _splittingService = splittingService;
        }

        public Task SendMessage(string message, MessageType messageType = MessageType.NormalText)
        {
            RestGuild guild = null;
            if (GuildId != default)
                guild = Server.GetGuild(GuildId).Result;
            var channel = (IRestMessageChannel)Server.GetChannel(ChannelId, guild).Result;

            foreach (var mess in _splittingService.SplitMessage(message, messageType))
            {
                channel.SendMessageAsync(mess);
                Log.Information("Bot sent message {splitted} {message}", mess, messageType != MessageType.NormalText ? "splitted" : string.Empty);
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
