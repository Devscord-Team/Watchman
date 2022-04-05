using System;
using System.Collections.Generic;

namespace Watchman.DomainModel.Configuration
{
    public abstract class MappedConfiguration<T> : IMappedConfiguration
    {
        public abstract T Value { get; set; }
        public ulong ServerId { get; }
        public string Name { get; }
        public abstract string Group { get; set; }
        public virtual string SubGroup { get; set; }

        public MappedConfiguration(ulong serverId)
        {
            this.Name = this.GetType().Name;
            this.ServerId = serverId;
        }

        public static bool operator ==(MappedConfiguration<T> x, MappedConfiguration<T> y) => Equals(x, y);
        public static bool operator !=(MappedConfiguration<T> x, MappedConfiguration<T> y) => !(x == y);

        public static new bool Equals(object obj1, object obj2)
        {
            return obj1 switch
            {
                null when obj2 == null => true,
                MappedConfiguration<T> mappedConfiguration1 => mappedConfiguration1.Equals(obj2),
                _ => false
            };
        }

        public override bool Equals(object obj)
        {
            return obj is MappedConfiguration<T> configuration
                   && EqualityComparer<T>.Default.Equals(this.Value, configuration.Value)
                   && this.ServerId == configuration.ServerId
                   && this.Name == configuration.Name
                   && this.Group == configuration.Group
                   && this.SubGroup == configuration.SubGroup;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.Value, this.ServerId, this.Name, this.Group, this.SubGroup);
        }
    }
}
