using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Serilog;
using Watchman.Cqrs;
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

        public async Task ScanChannelHistory(DiscordServerContext server, ChannelContext channel, DateTime? startTime = null) // startTime ->->-> now
        {
            const int LIMIT = 20000; // low limit to use props of startTime optimization

            if (channel.Name.Contains("logs"))
                return;
            
            startTime ??= DateTime.UnixEpoch;
            var messages = ReadMessages(server, channel, limit: 1);
            if (messages.Count == 0 || LastMessageIsOlderThanStartTime(messages, startTime.Value))
            {
                Log.Information($"Channel: {channel.Name} has no new messages");
                return;
            }

            var lastMessageId = 0UL;
            do
            {
                messages = ReadMessages(server, channel, LIMIT, lastMessageId);
                if (messages.Count == 0)
                {
                    break;
                }

                lastMessageId = messages.Last().Id;
                await SaveMessages(messages, channel.Id);

                if (this.LastMessageIsOlderThanStartTime(messages, startTime.Value))
                {
                    break;
                }

            } while (messages.Count == LIMIT);

            Log.Information($"Channel: {channel.Name} read and saved");
        }

        private async Task SaveMessages(IEnumerable<Message> messages, ulong channelId)
        {
            var convertedMessages = ConvertToMessages(messages);
            var command = new AddMessagesCommand(convertedMessages, channelId);
            await _commandBus.ExecuteAsync(command);
        }

        private IEnumerable<DomainModel.Messages.Message> ConvertToMessages(IEnumerable<Message> messages)
        {
            return messages.Select(x =>
            {
                var builder = DomainModel.Messages.Message.Create(x.Request.OriginalMessage)
                    .WithAuthor(x.Contexts.User.Id, x.Contexts.User.Name)
                    .WithChannel(x.Contexts.Channel.Id, x.Contexts.Channel.Name)
                    .WithServer(x.Contexts.Server.Id, x.Contexts.Server.Name, x.Contexts.Server.Owner.Id, x.Contexts.Server.Owner.Name)
                    .WithSentAtDate(x.Request.SentAt);

                return builder.Build();
            });
        }

        private List<Message> ReadMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong lastMessageId = 0)
        {
            var messages = lastMessageId == 0
                ? _messagesHistoryService.ReadMessagesAsync(server, channel, limit).Result.ToList()
                : _messagesHistoryService.ReadMessagesAsync(server, channel, limit, lastMessageId, goBefore: true).Result.ToList();

            return messages;
        }

        private bool LastMessageIsOlderThanStartTime(IEnumerable<Message> messages, DateTime startTime)
        {
            return messages.Last().Request.SentAt < startTime;
        }
    }
}
