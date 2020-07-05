using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public class DiscordServerContextFactory : IContextFactory<RestGuild, DiscordServerContext>
    {
        public DiscordServerContext Create(RestGuild restGuild)
        {
            var userFactory = new UserContextsFactory();
            var channelFactory = new ChannelContextFactory();

            var owner = userFactory.Create(restGuild.GetOwnerAsync().Result);
            var restTextChannel = restGuild.GetSystemChannelAsync().Result;
            var systemChannel = restTextChannel != null ? channelFactory.Create(restTextChannel) : null;
            var textChannels = restGuild.GetTextChannelsAsync().Result.Select(x => channelFactory.Create(x));

            return new DiscordServerContext(restGuild.Id, restGuild.Name, owner, systemChannel, textChannels);
        }
    }
}
