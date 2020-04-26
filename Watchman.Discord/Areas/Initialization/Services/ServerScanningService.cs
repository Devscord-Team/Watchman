using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Commands;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class ServerScanningService
    {
        private readonly ICommandBus _commandBus;
        private readonly MessagesHistoryService _messagesHistoryService;

        public ServerScanningService(ICommandBus commandBus, MessagesHistoryService messagesHistoryService)
        {
            _commandBus = commandBus;
            _messagesHistoryService = messagesHistoryService;
        }

        public async Task ScanChannelHistory(DiscordServerContext server, ChannelContext channel)
        {
            const int LIMIT = 10000;

            if (channel.Name.Contains("logs"))
                return;

            var messages = (await _messagesHistoryService.ReadMessagesAsync(server, channel, LIMIT)).ToList();
            Serilog.Log.Information($"Channel: {channel.Name} read");
            if (messages.Count == 0)
            {
                return;
            }

            var lastMessageId = messages.Last().Id;
            await SaveMessages(messages, channel.Id);
            Serilog.Log.Information($"Channel: {channel.Name} saved");

            do
            {
                messages = (await _messagesHistoryService.ReadMessagesAsync(server, channel, LIMIT, lastMessageId, goBefore: true)).ToList();
                Serilog.Log.Information($"Channel: {channel.Name} read");
                if (messages.Count == 0)
                {
                    break;
                }

                lastMessageId = messages.Last().Id;
                await SaveMessages(messages, channel.Id);
                Serilog.Log.Information($"Channel: {channel.Name} saved");

            } while (messages.Count < LIMIT);
        }

        private async Task SaveMessages(IEnumerable<Devscord.DiscordFramework.Services.Models.Message> messages, ulong channelId)
        {
            var convertedMessages = ConvertToMessages(messages);
            var command = new AddMessagesCommand(convertedMessages, channelId);
            await _commandBus.ExecuteAsync(command);
        }

        private IEnumerable<Message> ConvertToMessages(IEnumerable<Devscord.DiscordFramework.Services.Models.Message> messages)
        {
            return messages.Select(x =>
            {
                var builder = Message.Create(x.Request.OriginalMessage);
                builder.WithAuthor(x.Contexts.User.Id, x.Contexts.User.Name);
                builder.WithChannel(x.Contexts.Channel.Id, x.Contexts.Channel.Name);
                builder.WithServer(x.Contexts.Server.Id, x.Contexts.Server.Name, x.Contexts.Server.Owner.Id, x.Contexts.Server.Owner.Name);
                builder.WithSentAtDate(x.Request.SentAt);
                return builder.Build();
            });
        }
    }
}
