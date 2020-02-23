using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Integrations.MongoDB
{
    public abstract class Entity
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;
        public int Version { get; private set; }

        protected void Update()
        {
            UpdatedAt = DateTime.UtcNow;
            Version++;
        }
    }
}
