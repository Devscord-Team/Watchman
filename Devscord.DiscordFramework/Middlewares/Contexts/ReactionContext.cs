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
        public DateTime SentAt { get; }

        public ReactionContext(ChannelContext channelContext, UserContext userContext, MessageContext messageContext, string emoteName, DateTime sentAt)
        {
            this.ChannelContext = channelContext;
            this.UserContext = userContext;
            this.MessageContext = messageContext;
            this.EmoteName = emoteName;
            this.SentAt = sentAt;
        }
    }
}
