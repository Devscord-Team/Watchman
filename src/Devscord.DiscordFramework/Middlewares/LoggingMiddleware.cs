using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Discord.WebSocket;
using System;

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
