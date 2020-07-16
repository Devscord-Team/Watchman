using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ChannelContextFactory : IContextFactory<IChannel, ChannelContext>
    {
        public ChannelContext Create(IChannel restChannel)
        {
            return restChannel == null ? null : new ChannelContext(restChannel.Id, restChannel.Name);
        }
    }
}
