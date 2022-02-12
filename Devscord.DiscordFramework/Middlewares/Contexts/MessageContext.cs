using System;
using Devscord.DiscordFramework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class MessageContext : IDiscordContext
    {
        public DateTime SentAt { get; private set; }
        public bool IsBotCommand { get; private set; }

        public MessageContext(DateTime sentAt, bool isBotCommand)
        {
            this.SentAt = sentAt;
            this.IsBotCommand = isBotCommand;
        }
    }
}