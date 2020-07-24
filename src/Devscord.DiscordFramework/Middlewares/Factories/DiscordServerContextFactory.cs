using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class DiscordServerContextFactory : IContextFactory<IGuild, DiscordServerContext>
    {
        public DiscordServerContext Create(IGuild restGuild)
        {
            var userFactory = new UserContextsFactory();
            var channelFactory = new ChannelContextFactory();

            var owner = userFactory.Create(restGuild.GetOwnerAsync().Result);
            var systemChannel = channelFactory.Create(restGuild.GetSystemChannelAsync().Result);
            var textChannels = restGuild.GetTextChannelsAsync().Result.Select(x => channelFactory.Create(x));

            return new DiscordServerContext(restGuild.Id, restGuild.Name, owner, systemChannel, textChannels);
        }
    }
}
