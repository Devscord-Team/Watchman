using System;
using System.Collections.Generic;

namespace Watchman.DomainModel.Configuration
{
    public abstract class MappedConfiguration<T> : IMappedConfiguration
    {
        public abstract T Value { get; set; }
        public ulong ServerId { get; }
        public string Name { get; }

        public MappedConfiguration(ulong serverId)
        {
            this.Name = this.GetType().Name;
            this.ServerId = serverId;
        }

        public override bool Equals(object obj)
        {
            return obj is MappedConfiguration<T> configuration 
                   && EqualityComparer<T>.Default.Equals(this.Value, configuration.Value) 
                   && this.ServerId == configuration.ServerId 
                   && this.Name == configuration.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.ServerId, this.Name);
        }
    }
}
