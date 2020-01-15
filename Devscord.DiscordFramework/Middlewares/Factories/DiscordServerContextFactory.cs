using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class DiscordServerContextFactory : IContextFactory<SocketGuild, DiscordServerContext>
    {
        public DiscordServerContext Create(SocketGuild socketGuild)
        {
            var userFactory = new UserContextsFactory();
            var channelFactory = new ChannelContextFactory();

            var owner = userFactory.Create(socketGuild.Owner);
            var systemChannel = channelFactory.Create(socketGuild.SystemChannel);
            var textChannels = socketGuild.TextChannels.Select(x => channelFactory.Create(x));

            return new DiscordServerContext(socketGuild.Id, socketGuild.Name, owner, systemChannel, textChannels);
        }
    }
}
