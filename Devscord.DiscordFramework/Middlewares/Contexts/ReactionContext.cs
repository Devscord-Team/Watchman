using System;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class ReactionContext : IDiscordContext
    {
        public ulong ChannelId { get; }
        public string ChannelName { get; }
        public string EmoteName { get; }
        public string MessageContent { get; }
        public string MessageAuthorName { get; }
        public ulong MessageAuthorId { get; }
        public ulong MessageId { get; }
        public string UserName { get; }
        public ulong UserId { get; }
        public DateTime SentAt { get; }

        public ReactionContext(ulong channelId, string channelName, string emoteName, string messageContent, string messageAuthorName, ulong messageAuthorId, ulong messageId, string userName, ulong userId, DateTime sentAt)
        {
            this.ChannelId = channelId;
            this.ChannelName = channelName;
            this.EmoteName = emoteName;
            this.MessageContent = messageContent;
            this.MessageAuthorName = messageAuthorName;
            this.MessageAuthorId = messageAuthorId;
            this.MessageId = messageId;
            this.UserName = userName;
            this.UserId = userId;
            this.SentAt = sentAt;
        }
    }
}
