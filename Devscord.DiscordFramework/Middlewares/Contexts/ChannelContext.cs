using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class ChannelContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }

        public ChannelContext(ulong id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
