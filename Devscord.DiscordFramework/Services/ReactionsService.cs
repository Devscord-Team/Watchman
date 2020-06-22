using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Integration;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Services
{
    public class ReactionsService
    {
        public Task OnReactionAddedEvent(SocketReaction reaction)
        {
            return Task.Run(() => Server.UserAddedReaction(GetReactionContext(reaction)));
        }

        public Task OnReactionRemovedEvent(SocketReaction reaction)
        {
            return Task.Run(() => Server.UserRemovedReaction(GetReactionContext(reaction)));
        }

        private ReactionContext GetReactionContext(SocketReaction reaction)
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
