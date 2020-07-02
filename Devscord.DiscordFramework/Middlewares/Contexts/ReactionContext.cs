using System;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class ReactionContext : IDiscordContext
    {
        public ChannelContext ChannelContext { get; }
        public UserContext UserContext { get; }
        public MessageContext MessageContext { get; }
        public string EmoteName { get; }
        public DateTime ReactedAt { get; }

        public ReactionContext(ChannelContext channelContext, UserContext userContext, MessageContext messageContext, string emoteName, DateTime reactedAt)
        {
            this.ChannelContext = channelContext;
            this.UserContext = userContext;
            this.MessageContext = messageContext;
            this.EmoteName = emoteName;
            this.ReactedAt = reactedAt;
        }
    }
}
