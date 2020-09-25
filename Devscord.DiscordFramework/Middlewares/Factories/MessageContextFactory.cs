using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class MessageContextFactory : IContextFactory<IMessage, MessageContext>
    {
        private CommandParser _commandParser;

        public MessageContextFactory(CommandParser commandParser)
        {
            this._commandParser = commandParser;
        }

        public MessageContext Create(IMessage message)
        {
            bool isBotCommand = this._commandParser.Parse(message.Content, message.Timestamp.UtcDateTime).IsCommandForBot;
            return new MessageContext(message.Timestamp.UtcDateTime, isBotCommand);
        }
    }
}
