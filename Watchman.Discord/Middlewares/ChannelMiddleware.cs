using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;

namespace Watchman.Discord.Middlewares
{
    public class ChannelMiddleware : IMiddleware<ChannelContext>
    {
        public ChannelContext Process(SocketMessage data)
        {
            var channelContext = new ChannelContext(data.Channel.Id, data.Channel.Name);
            return channelContext;
        }
    }
}
