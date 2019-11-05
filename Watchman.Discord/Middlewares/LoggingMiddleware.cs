using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Watchman.Discord.Contexts;
using Watchman.Discord.Framework.Architecture.Middlewares;

namespace Watchman.Discord.Middlewares
{
    public class LoggingMiddleware : IMiddleware<EmptyContext>
    {
        public Task<IDiscordContext> Process(SocketMessage data)
        {
            ///todo add serilog
            Console.WriteLine($"{DateTime.Now} {data.Author}: {data.Content}");
            return default;
        }
    }
}
