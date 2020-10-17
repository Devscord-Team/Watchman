using System;

namespace Watchman.Integrations.Database
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
        public int Version { get; protected set; }

        protected void Update()
        {
            this.UpdatedAt = DateTime.UtcNow;
            this.Version++;
        }
    }
}
