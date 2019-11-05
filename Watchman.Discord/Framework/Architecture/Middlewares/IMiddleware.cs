using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Watchman.Discord.Framework.Architecture.Middlewares
{
    public interface IMiddleware<T> where T : IDiscordContext
    {
        Task<T> Process(SocketMessage data);
    }
}
