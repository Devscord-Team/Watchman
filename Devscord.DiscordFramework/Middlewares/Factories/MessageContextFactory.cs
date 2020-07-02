using Discord;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public class MessageContextFactory : IContextFactory<IUserMessage, MessageContext>
    {
        private readonly UserContextsFactory _userContextsFactory;

        public MessageContextFactory()
        {
            this._userContextsFactory = new UserContextsFactory();
        }

        public MessageContext Create(IUserMessage userMessage)
        {
            if (userMessage == null)
            {
                throw new ArgumentNullException();
            }

            var authorContext = this._userContextsFactory.Create(userMessage.Author);
            return new MessageContext(userMessage.Content, authorContext, userMessage.Id);
        }
    }
}
