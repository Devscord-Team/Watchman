using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord.Rest;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ChannelContextFactory : IContextFactory<IRestMessageChannel, ChannelContext>, IContextFactory<ISocketMessageChannel, ChannelContext>
    {
        public ChannelContext Create(IRestMessageChannel restChannel)
        {
            return restChannel == null ? null : new ChannelContext(restChannel.Id, restChannel.Name);
        }

        public ChannelContext Create(ISocketMessageChannel socketChannel)
        {
            return socketChannel == null ? null : new ChannelContext(socketChannel.Id, socketChannel.Name);
        }
    }
}
