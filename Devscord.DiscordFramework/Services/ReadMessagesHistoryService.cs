using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class ReadMessagesHistoryService
    {
        public async Task<IEnumerable<(Contexts contexts, DiscordRequest request, ulong messageId)>> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit)
        {
            var channelMessages = await Server.GetMessages(server, channelContext, limit);
            return channelMessages;
        }

        public async Task<IEnumerable<(Contexts contexts, DiscordRequest request, ulong messageId)>> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext, int limit, ulong fromMessageId, bool goBefore)
        {
            var channelMessages = await Server.GetMessages(server, channelContext, limit, fromMessageId, goBefore);
            return channelMessages;
        }
    }
}
