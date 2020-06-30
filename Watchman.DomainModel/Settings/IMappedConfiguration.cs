namespace Watchman.DomainModel.Settings
{
    public interface IMappedConfiguration
    {
    }

    public abstract class MappedConfiguration<T> : IMappedConfiguration
    {
        public T Value { get; }
        public ulong ServerId { get; }
        public abstract T DefaultValue { get; }

        public MappedConfiguration() => this.Value = this.DefaultValue;
    }
}
