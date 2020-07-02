using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class MessageContext : IDiscordContext
    {
        public string Content { get; }
        public UserContext AuthorContext { get; }
        public ulong Id { get; }

        public MessageContext(string content, UserContext authorContext, ulong id)
        {
            this.Content = content;
            this.AuthorContext = authorContext;
            this.Id = id;
        }
    }
}
