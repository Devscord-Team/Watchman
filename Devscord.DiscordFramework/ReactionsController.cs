using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Integration;
using Discord.WebSocket;

namespace Devscord.DiscordFramework
{
    public class ReactionsController
    {
        public Task OnReactionAddedEvent(SocketReaction reaction)
        {
            return Task.Run(() => Server.UserAddedReaction(GetReactionInformation(reaction)));
        }

        public Task OnReactionRemovedEvent(SocketReaction reaction)
        {
            return Task.Run(() => Server.UserRemovedReaction(GetReactionInformation(reaction)));
        }

        private ReactionContext GetReactionInformation(SocketReaction reaction)
        {
            return new ReactionContext
            {
                Channel = reaction.Channel,
                Emote = reaction.Emote,
                Message = reaction.Message,
                MessageId = reaction.MessageId,
                User = reaction.User,
                UserId = reaction.UserId,
                SentAt = DateTime.Now
            };
        }
    }
}
