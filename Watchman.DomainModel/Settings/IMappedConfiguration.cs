namespace Watchman.DomainModel.Settings
{
    public interface IMappedConfiguration
    {
        ulong ServerId { get; }
        string Name { get; }
    }

    public abstract class MappedConfiguration<T> : IMappedConfiguration
    {
        public T Value { get; }
        public ulong ServerId { get; }
        public abstract string Name { get; }
        public abstract T DefaultValue { get; }

        public MappedConfiguration() => this.Value = this.DefaultValue;
    }
}
