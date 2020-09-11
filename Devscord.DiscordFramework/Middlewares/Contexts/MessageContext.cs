using System;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class MessageContext : IDiscordContext
    {
        public DateTime SendAt { get; private set; }
        public bool IsBotCommand { get; private set; }

        public MessageContext(DateTime sendAt, bool botCommand)
        {
            this.SendAt = sendAt;
            this.IsBotCommand = botCommand;
        }
    }
}