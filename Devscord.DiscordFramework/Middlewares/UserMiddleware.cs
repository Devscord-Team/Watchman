using Autofac;
using Devscord.DiscordFramework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services;
using Discord;
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

        public IDiscordContext Process(IMessage data)
        {
            var user = data.Author;
            return this._userContextsFactory.Create(user);
        }
    }
}
