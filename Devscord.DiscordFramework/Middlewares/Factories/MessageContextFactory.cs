using Devscord.DiscordFramework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal interface IMessageContextFactory : IContextFactory<IMessage, MessageContext>
    {
    }

    internal class MessageContextFactory :  IMessageContextFactory
    {
        private CommandParser _commandParser;

        public MessageContextFactory(CommandParser commandParser)
        {
            this._commandParser = commandParser;
        }

        public MessageContext Create(IMessage message)
        {
            var isBotCommand = this._commandParser.Parse(message.Content, message.Timestamp.UtcDateTime).IsCommandForBot;
            return new MessageContext(message.Timestamp.UtcDateTime, isBotCommand);
        }
    }
}
