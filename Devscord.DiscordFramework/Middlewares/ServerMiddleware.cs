using Autofac;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ServerMiddleware : IMiddleware
    {
        private readonly DiscordServerContextFactory _discordServerContextFactory;

        public ServerMiddleware(UsersService usersService, UsersRolesService usersRolesService, IComponentContext context)
        {
            this._discordServerContextFactory = new DiscordServerContextFactory(usersService, usersRolesService, context.Resolve<UserContextsFactory>(), context.Resolve<ChannelContextFactory>());
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var serverInfo = ((SocketGuildChannel) data.Channel).Guild;
            var guild = Server.GetGuild(serverInfo.Id).Result;
            return this._discordServerContextFactory.Create(guild);
        }
    }
}
