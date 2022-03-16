using System;
using Devscord.DiscordFramework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class MessageContext : IDiscordContext
    {
        public ulong MessageId { get; private set; }
        public DateTime SentAt { get; private set; }
        public bool IsBotCommand { get; private set; }

        public MessageContext(ulong messageId, DateTime sentAt, bool isBotCommand)
        {
            this.MessageId = messageId;
            this.SentAt = sentAt;
            this.IsBotCommand = isBotCommand;
        }
    }
}