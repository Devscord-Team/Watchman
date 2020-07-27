using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class UserMiddleware : IMiddleware
    {
        private readonly UserContextsFactory _userContextsFactory;

        public UserMiddleware(UsersRolesService usersRolesService, DiscordServersService discordServersService)
        {
            this._userContextsFactory = new UserContextsFactory(usersRolesService, discordServersService);
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var user = (SocketGuildUser) data.Author;
            return this._userContextsFactory.Create(user);
        }
    }
}
