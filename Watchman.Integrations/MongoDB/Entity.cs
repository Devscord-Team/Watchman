using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Integrations.MongoDB
{
    public abstract class Entity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;
        public int Version { get; protected set; }

        protected void Update()
        {
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }
}
