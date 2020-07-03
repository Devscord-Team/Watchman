using Discord;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class MessageContextFactory : IContextFactory<IUserMessage, MessageContext>
    {
        private readonly UserContextsFactory _userContextsFactory;

        public MessageContextFactory(UserContextsFactory userContextsFactory)
        {
            this._userContextsFactory = userContextsFactory;
        }

        public MessageContext Create(IUserMessage userMessage)
        {
            var authorContext = this._userContextsFactory.Create(userMessage.Author);
            return new MessageContext(userMessage.Id, userMessage.Content, authorContext);
        }
    }
}
