using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Areas.Reactions.Contexts;

namespace Watchman.Discord.Areas.Reactions.Services
{
    public static class ReactionsService
    {
        public static Action<ReactionContext> UserAddedReaction { get; set; }
        public static Action<ReactionContext> UserRemovedReaction { get; set; }

        public static Task OnReactionAddedEvent(SocketReaction reaction)
        {
            return Task.Run(() => UserAddedReaction(GetReactionContext(reaction)));
        }

        public static Task OnReactionRemovedEvent(SocketReaction reaction)
        {
            return Task.Run(() => UserRemovedReaction(GetReactionContext(reaction)));
        }

        private static ReactionContext GetReactionContext(SocketReaction reaction)
        {
            return new ReactionContext(
                reaction.Channel,
                reaction.Emote,
                reaction.Message.GetValueOrDefault(),
                reaction.MessageId,
                reaction.User.GetValueOrDefault(),
                reaction.UserId,
                sentAt: DateTime.Now);
        }
    }
}
