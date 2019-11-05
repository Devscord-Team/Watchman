using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;

namespace Watchman.Discord.Middlewares
{
    public class UserMiddleware : IMiddleware<UserContext>
    {
        public UserContext Process(SocketMessage data)
        {
            return new UserContext(data.Author.Id, data.Author.ToString());
        }
    }
}
