using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public interface IChannelContextFactory : IContextFactory<IChannel, ChannelContext>
    {
    }

    internal class ChannelContextFactory : IChannelContextFactory
    {
        public ChannelContext Create(IChannel restChannel)
        {
            return restChannel == null ? null : new ChannelContext(restChannel.Id, restChannel.Name);
        }
    }
}
