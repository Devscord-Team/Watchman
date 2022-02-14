using Autofac;
using Devscord.DiscordFramework.Architecture.Middlewares;
using Devscord.DiscordFramework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    class MessageMiddleware : IMiddleware
    {
        private readonly MessageContextFactory _messageContextFactory;

        public MessageMiddleware(IComponentContext contex)
        {
            this._messageContextFactory = new MessageContextFactory(contex.Resolve<CommandParser>());
        }

        public IDiscordContext Process(IMessage data)
        {
            return this._messageContextFactory.Create(data);
        }
    }
}
