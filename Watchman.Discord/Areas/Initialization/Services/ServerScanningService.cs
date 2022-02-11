using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Extensions;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages.Commands;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class ServerScanningService
    {
        private UserContext BotContext => this._botContext ??= this._usersService.GetBot();

        private readonly ICommandBus _commandBus;
        private readonly IMessagesHistoryService _messagesHistoryService;
        private readonly UsersService _usersService;
        private UserContext _botContext;

        public ServerScanningService(ICommandBus commandBus, IMessagesHistoryService messagesHistoryService, UsersService usersService)
        {
            this._commandBus = commandBus;
            this._messagesHistoryService = messagesHistoryService;
            this._usersService = usersService;
        }

        public async Task ScanChannelHistory(DiscordServerContext server, ChannelContext channel, DateTime? startTime = null) // startTime ->->-> now
        {
            const int LIMIT = 20000; // low limit to use props of startTime optimization

            if (channel.Name.Contains("logs"))
            {
                return;
            }
            Log.Information("Reading channel {channel} ", channel.ToJson());

            startTime ??= DateTime.UnixEpoch;
            var messages = await this.ReadMessagesAsync(server, channel, limit: 1).ToListAsync();
            if (!messages.Any() || this.LastMessageIsOlderThanStartTime(messages, startTime.Value))
            {
                Log.Information("Channel: {channel} has no new messages", channel.Name);
                return;
            }

            var lastMessageId = 0UL;
            do
            {
                messages = await this.ReadMessagesAsync(server, channel, LIMIT, lastMessageId).ToListAsync();
                if (!messages.Any())
                {
                    break;
                }
                lastMessageId = messages.Last().Id;
                await this.SaveMessages(messages, channel.Id);
                if (this.LastMessageIsOlderThanStartTime(messages, startTime.Value))
                {
                    break;
                }
            } while (messages.Count == LIMIT);

            Log.Information("Channel: {channel} read and saved", channel.Name);
        }

        private async Task SaveMessages(IEnumerable<Message> messages, ulong channelId)
        {
            var convertedMessages = this.ConvertToMessages(messages);
            var command = new AddMessagesCommand(convertedMessages, channelId);
            await this._commandBus.ExecuteAsync(command);
        }

        private IEnumerable<DomainModel.Messages.Message> ConvertToMessages(IEnumerable<Message> messages)
        {
            return messages.Select(x =>
            {
                return DomainModel.Messages.Message
                    .Create(x.Request.OriginalMessage)
                    .WithAuthor(x.Contexts.User.Id, x.Contexts.User.Name)
                    .WithChannel(x.Contexts.Channel.Id, x.Contexts.Channel.Name)
                    .WithServer(x.Contexts.Server.Id, x.Contexts.Server.Name)
                    .WithSentAtDate(x.Request.SentAt)
                    .Build();
            });
        }

        private async IAsyncEnumerable<Message> ReadMessagesAsync(DiscordServerContext server, ChannelContext channel, int limit, ulong lastMessageId = 0)
        {
            var messages = lastMessageId == 0
                ? this._messagesHistoryService.ReadMessagesAsync(server, channel, limit)
                : this._messagesHistoryService.ReadMessagesAsync(server, channel, limit, lastMessageId, goBefore: true);
            await foreach (var message in messages)
            {
                if (message.Contexts.User.Id != this.BotContext.Id)
                {
                    yield return message;
                }
            }
        }

        private bool LastMessageIsOlderThanStartTime(IEnumerable<Message> messages, DateTime startTime)
        {
            return messages.Last().Request.SentAt < startTime;
        }
    }
}
