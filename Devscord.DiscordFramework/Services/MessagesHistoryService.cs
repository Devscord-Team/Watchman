using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;

namespace Devscord.DiscordFramework.Services
{
    public class MessagesHistoryService
    {
        public async Task<IEnumerable<Message>> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit)
        {
            var channelMessages = await Server.GetMessages(server, channelContext, limit);
            return channelMessages;
        }

        public async Task<IEnumerable<Message>> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit, ulong fromMessageId, bool goBefore)
        {
            var channelMessages = await Server.GetMessages(server, channelContext, limit, fromMessageId, goBefore);
            return channelMessages;
        }
    }
}
