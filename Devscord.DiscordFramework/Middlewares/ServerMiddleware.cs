using Autofac;
using Devscord.DiscordFramework.Architecture.Middlewares;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ServerMiddleware : IMiddleware
    {
        private readonly IDiscordServerContextFactory _discordServerContextFactory;

        public ServerMiddleware(IDiscordServerContextFactory discordServerContextFactory)
        {
            this._discordServerContextFactory = discordServerContextFactory;
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var serverInfo = ((SocketGuildChannel) data.Channel).Guild;
            var guild = Server.GetGuild(serverInfo.Id).Result;
            return this._discordServerContextFactory.Create(guild);
        }
    }
}
