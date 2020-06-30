using System;
using Discord;
using Discord.WebSocket;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ReactionContextFactory : IContextFactory<Tuple<SocketReaction, IUserMessage>, ReactionContext>
    {
        public ReactionContext Create(Tuple<SocketReaction, IUserMessage> reactionAndUserMessage)
        {
            var reaction = reactionAndUserMessage.Item1;
            var userMessage = reactionAndUserMessage.Item2;

            var channelContext = new ChannelContextFactory().Create(reaction.Channel);
            var userContext = new UserContextsFactory().Create(reaction.User.Value);
            var messageContext = new MessageContextFactory().Create(userMessage);

            return new ReactionContext(channelContext, userContext, messageContext, reaction.Emote.Name, sentAt: DateTime.Now);
        }
    }
}
