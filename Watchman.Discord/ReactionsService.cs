using System;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Watchman.Discord
{
    public class ReactionsService
    {
        private Action<ReactionContext> _userAddedReaction;
        private Action<ReactionContext> _userRemovedReaction;

        public void OnUserAddedReaction(ReactionContext context)
        {
            _userAddedReaction(context);
        }

        public void OnUserRemovedReaction(ReactionContext context)
        {
            _userRemovedReaction(context);
        }

        public void AddUserAddedReaction(Action<ReactionContext> action)
        {
            _userAddedReaction += action;
        }

        public void AddUserRemovedReaction(Action<ReactionContext> action)
        {
            _userRemovedReaction += action;
        }
    }
}
