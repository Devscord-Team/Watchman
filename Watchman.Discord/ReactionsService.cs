using System;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord
{
    public class ReactionsService
    {
        public Action<ReactionContext> OnUserAddedReaction { get; private set; }
        public Action<ReactionContext> OnUserRemovedReaction { get; private set; }

        public void AddOnUserAddedReaction(Action<ReactionContext> action)
        {
            OnUserAddedReaction += action;
        }

        public void AddOnUserRemovedReaction(Action<ReactionContext> action)
        {
            OnUserRemovedReaction += action;
        }
    }
}
