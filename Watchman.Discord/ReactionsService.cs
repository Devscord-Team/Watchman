using System;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord
{
    public static class ReactionsService
    {
        public static Action<ReactionContext> UserAddedReaction { get; set; }
        public static Action<ReactionContext> UserRemovedReaction { get; set; }
    }
}
