using Devscord.DiscordFramework.Framework.Architecture.Middlewares;

namespace Devscord.DiscordFramework.Middlewares.Contexts
{
    public class DiscordServerContext : IDiscordContext
    {
        public ulong Id { get; private set; }
        public string Name { get; private set; }
        public UserContext Owner { get; private set; }

        public DiscordServerContext(ulong id, string name, UserContext owner)
        {
            Id = id;
            Name = name;
            Owner = owner;
        }
    }
}
