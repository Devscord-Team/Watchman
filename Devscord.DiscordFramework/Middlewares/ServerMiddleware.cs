using Autofac;
using Devscord.DiscordFramework.Architecture.Middlewares;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ServerMiddleware : IMiddleware
    {
        private readonly DiscordServerContextFactory _discordServerContextFactory;

        public ServerMiddleware(IComponentContext context)
        {
            this._discordServerContextFactory = new DiscordServerContextFactory(context);
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var serverInfo = ((SocketGuildChannel) data.Channel).Guild;
            var guild = Server.GetGuild(serverInfo.Id).Result;
            return this._discordServerContextFactory.Create(guild);
        }
    }
}
