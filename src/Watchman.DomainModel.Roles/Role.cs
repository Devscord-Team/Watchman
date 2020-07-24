using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.DiscordServer
{
    public class Role : Entity, IAggregateRoot
    {
        public string Name { get; private set; }
        public ulong ServerId { get; private set; }

        public Role(string name, ulong serverId)
        {
            this.Name = name;
            this.ServerId = serverId;
        }
    }
}
