using System;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework
{
    public class ReactionContext
    {
        public ISocketMessageChannel ChannelContext { get; }
        public IEmote Emote { get; }
        public SocketUserMessage Message { get; }
        public ulong MessageId { get; }
        public IUser UserContext { get; }
        public ulong UserId { get; }
        public DateTime SentAt { get; }

        public ReactionContext(ISocketMessageChannel channelContext, IEmote emote, SocketUserMessage message, ulong messageId, IUser userContext, ulong userId, DateTime sentAt)
        {
            this.ChannelContext = channelContext;
            this.Emote = emote;
            this.Message = message; 
            this.MessageId = messageId;
            this.UserContext = userContext;
            this.UserId = userId;
            this.SentAt = sentAt;
        }
    }
}