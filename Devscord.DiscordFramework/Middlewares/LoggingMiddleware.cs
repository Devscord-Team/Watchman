using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Middlewares
{
    public class LoggingMiddleware : IMiddleware
    {
        public IDiscordContext Process(SocketMessage data)
        {
            ///todo add serilog
            Console.WriteLine($"{DateTime.Now} {data.Author}: {data.Content}");
            return default;
        }
    }
}
