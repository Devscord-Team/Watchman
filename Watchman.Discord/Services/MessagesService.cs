using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Discord.Framework;

namespace Watchman.Discord.Services
{
    public class MessagesService
    {
        public ulong DefaultChannelId { get; set; }

        public Task SendMessage(string message, ulong channelId = 0)
        {
            if(channelId == 0)
            {
                channelId = this.DefaultChannelId;
            }

            var channel = (ISocketMessageChannel)Server.GetChannel(channelId);
            return channel.SendMessageAsync(message);
        }

        public Task SendFile(string filePath, ulong channelId = 0)
        {
            if (channelId == 0)
            {
                channelId = this.DefaultChannelId;
            }

            var channel = (ISocketMessageChannel)Server.GetChannel(channelId);
            return channel.SendFileAsync(filePath);
        }
    }
}
