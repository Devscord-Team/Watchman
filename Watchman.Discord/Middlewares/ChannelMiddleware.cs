using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;

namespace Watchman.Discord.Middlewares
{
    public class ChannelMiddleware : IMiddleware<ChannelContext>
    {
        public Task<IDiscordContext> Process(SocketMessage data)
        {
            throw new NotImplementedException();
        }
    }
}
