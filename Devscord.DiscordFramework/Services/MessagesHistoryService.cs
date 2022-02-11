using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using System.Collections.Generic;

namespace Devscord.DiscordFramework.Services
{
    public interface IMessagesHistoryService
    {
        IAsyncEnumerable<Message> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit);
        IAsyncEnumerable<Message> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit, ulong fromMessageId, bool goBefore);
    }

    public class MessagesHistoryService
    {
        public IAsyncEnumerable<Message> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit)
        {
            return Server.GetMessages(server, channelContext, limit);
        }

        public IAsyncEnumerable<Message> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit, ulong fromMessageId, bool goBefore)
        {
            return Server.GetMessages(server, channelContext, limit, fromMessageId, goBefore);
        }
    }
}
