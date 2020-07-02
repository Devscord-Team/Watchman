using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ChannelContextFactory : IContextFactory<IRestMessageChannel, ChannelContext>
    {
        public ChannelContext Create(IRestMessageChannel restChannel)
        {
            return restChannel == null ? null : new ChannelContext(restChannel.Id, restChannel.Name);
        }
    }
}
