using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class MessageContext : IDiscordContext
    {
        public ulong Id { get; }
        public string Content { get; }
        public UserContext AuthorContext { get; }

        public MessageContext(ulong id, string content, UserContext authorContext)
        {
            this.Id = id;
            this.Content = content;
            this.AuthorContext = authorContext;         
        }
    }
}
