using Autofac;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.WebSocket;

namespace Devscord.DiscordFramework.Middlewares
{
    class MessageMiddleware : IMiddleware
    {
        private readonly MessageContextFactory _messageContextFactory;

        public MessageMiddleware(IComponentContext contex)
        {
            this._messageContextFactory = new MessageContextFactory(contex);
        }

        public IDiscordContext Process(SocketMessage data)
        {
            return this._messageContextFactory.Create(data.Timestamp.UtcDateTime, data.Content);
        }
    }
}
