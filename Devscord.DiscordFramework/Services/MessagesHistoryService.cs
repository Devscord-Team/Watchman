using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Services
{
    public class MessagesHistoryService
    {
        public async IAsyncEnumerable<Message> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit)
        {
            await foreach (var message in Server.GetMessages(server, channelContext, limit))
            {
                yield return message;
            }
        }

        public async IAsyncEnumerable<Message> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit, ulong fromMessageId, bool goBefore)
        {
            await foreach (var message in Server.GetMessages(server, channelContext, limit, fromMessageId, goBefore))
            {
                yield return message;
            }
        }
    }
}
