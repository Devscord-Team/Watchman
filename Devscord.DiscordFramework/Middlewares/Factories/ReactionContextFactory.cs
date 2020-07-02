using System;
using Discord;
using Discord.WebSocket;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ReactionContextFactory : IContextFactory<Tuple<SocketReaction, IUserMessage>, ReactionContext>
    {
        private readonly ChannelContextFactory _channelContextFactory;
        private readonly UserContextsFactory _userContextsFactory;
        private readonly MessageContextFactory _messageContextFactory;

        public ReactionContextFactory()
        {
            this._channelContextFactory = new ChannelContextFactory();
            this._userContextsFactory = new UserContextsFactory();
            this._messageContextFactory = new MessageContextFactory();
        }

        public ReactionContext Create(Tuple<SocketReaction, IUserMessage> reactionAndUserMessage)
        {
            var reaction = reactionAndUserMessage.Item1;
            var userMessage = reactionAndUserMessage.Item2;

            var channelContext = this._channelContextFactory.Create(reaction.Channel);
            var userContext = this._userContextsFactory.Create(reaction.User.Value);
            var messageContext = this._messageContextFactory.Create(userMessage);

            return new ReactionContext(channelContext, userContext, messageContext, reaction.Emote.Name, reactedAt: DateTime.Now);
        }
    }
}
