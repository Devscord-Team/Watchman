using System;

namespace Devscord.DiscordFramework.Services.Models
{
    public readonly struct SmallMessage
    {
        public string Content { get; }
        public ulong UserId { get; }
        public DateTime SentAt { get; }
        public ulong ServerId { get; }

        public SmallMessage(string content, ulong userId, DateTime sentAt, ulong serverId)
        {
            this.Content = content;
            this.UserId = userId;
            this.SentAt = sentAt;
            this.ServerId = serverId;
        }
    }
}
