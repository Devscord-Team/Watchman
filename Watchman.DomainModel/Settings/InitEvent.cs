using System;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings
{
    public class InitEvent : Entity
    {
        public ulong ServerId { get; set; }
        public DateTime EndedAt { get; set; }
    }
}
