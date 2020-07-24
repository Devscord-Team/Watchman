using Devscord.EventStore;

namespace Watchman.DomainModel.ServerPrefixes.Events
{
    public class PrefixRemovedFromServerEvent : Event
    {
        public ulong ServerId { get; set; }
        public string Prefix { get; set; }

        public PrefixRemovedFromServerEvent()
        {
        }

        public PrefixRemovedFromServerEvent(ulong serverId, string prefix)
        {
            this.ServerId = serverId;
            this.Prefix = prefix;
        }
    }
}
