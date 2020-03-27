using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ChannelContextFactory : IContextFactory<IRestMessageChannel, ChannelContext>
    {
        public ChannelContext Create(IRestMessageChannel restChannel)
        {
            return new ChannelContext(restChannel.Id, restChannel.Name);
        }
    }
}
