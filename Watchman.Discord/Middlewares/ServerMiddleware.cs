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
        public DiscordServerContext Process(SocketMessage data)
        {
            var serverInfo = ((SocketGuildChannel)data.Channel).Guild;
            return new DiscordServerContext(serverInfo.Id, serverInfo.Name);
        }
    }
}
