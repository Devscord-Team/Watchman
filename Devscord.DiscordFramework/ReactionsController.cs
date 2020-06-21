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
            return new ReactionContext(
                reaction.Channel, 
                reaction.Emote, 
                reaction.Message, 
                reaction.MessageId, 
                reaction.User, 
                reaction.UserId, 
                sentAt: DateTime.Now);
        }
    }
}
