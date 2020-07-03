using System;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class ReactionContext : IDiscordContext
    {
        public Contexts Contexts { get; }
        public MessageContext MessageContext { get; }
        public string EmoteName { get; }
        public DateTime ReactedAt { get; }

        public ReactionContext(Contexts contexts, MessageContext messageContext, string emoteName, DateTime reactedAt)
        {
            this.Contexts = contexts;
            this.MessageContext = messageContext;
            this.EmoteName = emoteName;
            this.ReactedAt = reactedAt;
        }
    }
}
