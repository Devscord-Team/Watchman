using System;

namespace Devscord.DiscordFramework.Services.Models
{
    public class SmallMessage
    {
        public string Content { get; set; }
        public ulong UserId { get; set; }
        public DateTime SentAt { get; set; }

        public SmallMessage(string content, ulong userId, DateTime sentAt)
        {
            Content = content;
            UserId = userId;
            SentAt = sentAt;
        }
    }
}
