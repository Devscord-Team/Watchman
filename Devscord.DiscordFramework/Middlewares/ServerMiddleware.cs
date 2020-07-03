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
        private readonly RestGuildGetterService _restGuildGetterService; 

        public ServerMiddleware()
        {
            this._discordServerContextsFactory = new DiscordServerContextFactory();
            this._restGuildGetterService = new RestGuildGetterService();
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var guild = this._restGuildGetterService.GetRestGuild(data.Channel);
            return this._discordServerContextsFactory.Create(guild);
        }
    }
}
