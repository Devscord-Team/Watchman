using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ChannelContextFactory : IContextFactory<IMessageChannel, ChannelContext>
    {
        public ChannelContext Create(IMessageChannel messageChannel)
        {
            return messageChannel == null ? null : new ChannelContext(messageChannel.Id, messageChannel.Name);
        }
    }
}
