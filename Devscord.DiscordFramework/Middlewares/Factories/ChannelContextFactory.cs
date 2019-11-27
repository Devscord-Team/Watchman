using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    internal class ChannelContextFactory : IContextFactory<ISocketMessageChannel, ChannelContext>
    {
        public ChannelContext Create(ISocketMessageChannel socketChannel)
        {
            return new ChannelContext(socketChannel.Id, socketChannel.Name);
        }
    }
}
