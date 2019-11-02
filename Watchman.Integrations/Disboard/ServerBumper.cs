using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace Watchman.Integrations.Disboard
{
    public class ServerBumper
    {
        private const string BumpMessage = "!d bump";

        private readonly ISocketMessageChannel _channel;

        public ServerBumper(ISocketMessageChannel channel)
        {
            _channel = channel;
        }

        public Task SendBump()
        {
            this._channel.SendMessageAsync(BumpMessage);
            return Task.CompletedTask;
        }
    }
}
