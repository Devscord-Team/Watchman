using System;
using Discord;
using Discord.WebSocket;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class ReactionContextFactory : IContextFactory<SocketReaction, IUserMessage, ReactionContext>
    {
        public ReactionContext Create(SocketReaction reaction, IUserMessage userMessage)
        {
            return new ReactionContext(
                reaction.Channel.Id,
                reaction.Channel.Name,
                reaction.Emote.Name,
                userMessage.Content,
                userMessage.Author.Username,
                userMessage.Author.Id,
                reaction.MessageId,
                reaction.User.Value.Username,
                reaction.UserId,
                sentAt: DateTime.Now);
        }
    }
}
