using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using System;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ChannelContextFactory : IContextFactory<IMessageChannel, ChannelContext>
    {
        public ChannelContext Create(IMessageChannel messageChannel)
        {
            return new ChannelContext(messageChannel.Id, messageChannel.Name);
        }
    }
}
