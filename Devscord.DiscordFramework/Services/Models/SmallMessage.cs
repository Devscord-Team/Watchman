using System;

namespace Devscord.DiscordFramework.Services.Models
{
    public readonly struct SmallMessage
    {
        public string Content { get; }
        public ulong UserId { get; }
        public DateTime SentAt { get; }

        public SmallMessage(string content, ulong userId, DateTime sentAt)
        {
            Content = content;
            UserId = userId;
            SentAt = sentAt;
        }
    }
}
