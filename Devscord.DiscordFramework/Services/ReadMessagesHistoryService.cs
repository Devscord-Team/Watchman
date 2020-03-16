using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Services
{
    public class ReadMessagesHistoryService
    {
        public async Task<IEnumerable<(Contexts contexts, DiscordRequest request)>> ReadMessagesAsync(DiscordServerContext server, ChannelContext channelContext)
        {
            var channelMessages = await Server.GetMessages(server, channelContext);
            return channelMessages;
        }
    }
}
