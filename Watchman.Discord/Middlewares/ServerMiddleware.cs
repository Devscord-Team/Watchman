using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;

namespace Watchman.Discord.Middlewares
{
    public class ServerMiddleware : IMiddleware<DiscordServerContext>
    {
        public Task<DiscordServerContext> Process(SocketMessage data)
        {
            throw new NotImplementedException();
        }
    }
}
