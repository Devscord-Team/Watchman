using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ServerMiddleware : IMiddleware<DiscordServerContext>
    {
        private readonly UserContextsFactory userContextsFactory;
        private readonly DiscordServerContextFactory discordServerContextsFactory;

        public ServerMiddleware()
        {
            this.userContextsFactory = new UserContextsFactory();
            this.discordServerContextsFactory = new DiscordServerContextFactory();
        }

        public DiscordServerContext Process(SocketMessage data)
        {
            var serverInfo = ((SocketGuildChannel)data.Channel).Guild;
            return this.discordServerContextsFactory.Create(serverInfo);
        }
    }
}
