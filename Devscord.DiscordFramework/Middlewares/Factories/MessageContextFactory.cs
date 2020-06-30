using Discord;
using Devscord.DiscordFramework.Middlewares.Contexts;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    public class MessageContextFactory : IContextFactory<IUserMessage, MessageContext>
    {
        public MessageContext Create(IUserMessage userMessage)
        {
            return userMessage == null ? null : new MessageContext(userMessage.Content, userMessage.Author.Username, userMessage.Author.Id, userMessage.Id);
        }
    }
}
