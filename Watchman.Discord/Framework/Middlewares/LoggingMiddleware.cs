using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Watchman.Discord.Framework.Middlewares
{
    public class LoggingMiddleware : IMiddleware<SocketMessage>
    {
        public Task Process(SocketMessage data)
        {
            Console.WriteLine($"{DateTime.Now} {data.Author}: {data.Content}");

            return Task.CompletedTask;
        }
    }
}
