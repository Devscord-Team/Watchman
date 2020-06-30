using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class MessageContext : IDiscordContext
    {
        public string Content { get; }
        public string AuthorName { get; }
        public ulong AuthorId { get; }
        public ulong Id { get; }

        public MessageContext(string content, string authorName, ulong authorId, ulong Id)
        {
            this.Content = content;
            this.AuthorName = authorName;
            this.AuthorId = authorId;
            this.Id = Id;
        }
    }
}
