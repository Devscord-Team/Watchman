using System;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework
{
    public class ReactionContext
    {
        public ISocketMessageChannel Channel { get; }
        public IEmote Emote { get; }
        public Optional<SocketUserMessage> Message { get; }
        public ulong MessageId { get; }
        public Optional<IUser> User { get; }
        public ulong UserId { get; }
        public DateTime SentAt { get; }

        public ReactionContext(ISocketMessageChannel channel, IEmote emote, Optional<SocketUserMessage> message, ulong messageId, Optional<IUser> user, ulong userId, DateTime sentAt)
        {
            this.Channel = channel;
            this.Emote = emote;
            this.Message = message; 
            this.MessageId = messageId;
            this.User = user;
            this.UserId = userId;
            this.SentAt = sentAt;
        }
    }
}