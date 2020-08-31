using Autofac;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    class MessageContextFactory
    {
        private CommandParser _commandParser;

        public MessageContextFactory(IComponentContext context)
        {
            this._commandParser = context.Resolve<CommandParser>();
        }

        public MessageContext Create(DateTime sendAt, string message)
        {
            bool IsBotCommand() => this._commandParser.Parse(message, sendAt).IsCommandForBot;
            return new MessageContext(sendAt, IsBotCommand);
        }
    }
}
