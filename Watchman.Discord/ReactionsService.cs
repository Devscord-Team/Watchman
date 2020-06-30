using System;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord
{
    public class ReactionsService
    {
        public Action<ReactionContext> UserAddedReaction { get; set; }
        public Action<ReactionContext> UserRemovedReaction { get; set; }
    }
}
