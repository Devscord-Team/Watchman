using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.Rest;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ServerMiddleware : IMiddleware
    {
        private readonly DiscordServerContextFactory _discordServerContextsFactory;

        public ServerMiddleware()
        {
            this._discordServerContextsFactory = new DiscordServerContextFactory();
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var guild = Server.GetGuild(data.Channel).Result;
            return this._discordServerContextsFactory.Create(guild);
        }
    }
}
