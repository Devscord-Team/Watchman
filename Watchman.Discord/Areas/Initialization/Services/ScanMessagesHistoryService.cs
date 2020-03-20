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
    public class ScanMessagesHistoryService
    {
        private readonly ICommandBus _commandBus;
        private readonly ReadMessagesHistoryService _readMessagesHistoryService;

        public ScanMessagesHistoryService(ICommandBus commandBus, ReadMessagesHistoryService readMessagesHistoryService)
        {
            _commandBus = commandBus;
            _readMessagesHistoryService = readMessagesHistoryService;
        }

        public async Task ScanChannelHistory(DiscordServerContext server, ChannelContext channel)
        {
            const int LIMIT = 1000;

            if (channel.Name.Contains("logs"))
                return;

            var messages = (await _readMessagesHistoryService.ReadMessagesAsync(server, channel, LIMIT)).ToList();
            if (messages.Count == 0)
            {
                return;
            }

            var lastMessageId = messages.Last().Id;
            await SaveMessages(messages, channel.Id);

            do
            {
                messages = (await _readMessagesHistoryService.ReadMessagesAsync(server, channel, LIMIT, lastMessageId, goBefore: true)).ToList();
                if (messages.Count == 0)
                {
                    break;
                }

                lastMessageId = messages.Last().Id;
                await SaveMessages(messages, channel.Id);

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
