using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    internal class DiscordServerContextFactory : IContextFactory<SocketGuild, DiscordServerContext>
    {
        public DiscordServerContext Create(SocketGuild socketGuild)
        {
            var userFactory = new UserContextsFactory();
            var channelFactory = new ChannelContextFactory();

            var owner = userFactory.Create(socketGuild.Owner);
            var defaultChannel = channelFactory.Create(socketGuild.DefaultChannel);

            return new DiscordServerContext(socketGuild.Id, socketGuild.Name, owner, defaultChannel);
        }
    }
}
