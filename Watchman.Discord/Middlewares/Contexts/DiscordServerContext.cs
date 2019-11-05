using Watchman.Discord.Framework.Architecture.Middlewares;

namespace Watchman.Discord.Middlewares.Contexts
{
    public class DiscordServerContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public DiscordServerContext(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
