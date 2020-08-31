using System;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class MessageContext : IDiscordContext
    {
        public DateTime SendAt { get; private set; }
        public bool IsBotCommandTest { get; private set; }

        private readonly Func<bool> _getIsBotCommand;

        public MessageContext(DateTime sendAt, Func<bool> botCommand)
        {
            this.SendAt = sendAt;
            this._getIsBotCommand = botCommand;
            this.IsBotCommandTest = this.IsBotCommand();
        }
        public bool IsBotCommand() => this._getIsBotCommand.Invoke();
    }
}