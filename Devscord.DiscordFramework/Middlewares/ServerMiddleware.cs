using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Integration;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    public class ServerMiddleware : IMiddleware
    {
        private readonly DiscordServerContextFactory discordServerContextsFactory;

        public ServerMiddleware()
        {
            this.discordServerContextsFactory = new DiscordServerContextFactory();
        }

        public IDiscordContext Process(SocketMessage data)
        {
            var serverInfo = ((SocketGuildChannel) data.Channel).Guild;
            var guild = Server.GetGuild(serverInfo.Id).Result;
            return this.discordServerContextsFactory.Create(guild);
        }
    }
}
