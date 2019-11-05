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
        public ulong DefaultChannel { get; set; }

        public Task SendMessage(string message, ulong channelId = 0)
        {
            if(channelId == 0)
            {
                channelId = this.DefaultChannel;
            }

            var channel = (ISocketMessageChannel)Server.GetChannel(channelId);
            channel.SendMessageAsync(message);

            return Task.CompletedTask;
        }

        public Task SendFile(string filePath, ulong channelId = 0)
        {
            if (channelId == 0)
            {
                channelId = this.DefaultChannel;
            }

            var channel = (ISocketMessageChannel)Server.GetChannel(channelId);
            channel.SendFileAsync(filePath);

            return Task.CompletedTask;
        }
    }
}
