﻿using Autofac;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing;
using Devscord.DiscordFramework.Middlewares.Factories;
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

        public IDiscordContext Process(SocketMessage data)
        {
            return this._messageContextFactory.Create(data);
        }
    }
}
