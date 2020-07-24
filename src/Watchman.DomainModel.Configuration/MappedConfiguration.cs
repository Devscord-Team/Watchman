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
    }
}
