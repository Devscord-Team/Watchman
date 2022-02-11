using Autofac;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class UserMiddleware : IMiddleware
    {
        private readonly IUserContextsFactory _userContextsFactory;

        public UserMiddleware(IUserContextsFactory userContextsFactory)
        {
            this._userContextsFactory = userContextsFactory;
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var user = (SocketGuildUser) data.Author;
            return this._userContextsFactory.Create(user);
        }
    }
}
