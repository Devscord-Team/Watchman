namespace Watchman.DomainModel.Settings
{
    public interface IMappedConfiguration
    {
        ulong ServerId { get; set; }
        string Name { get; }
    }

    public abstract class MappedConfiguration<T> : IMappedConfiguration
    {
        public T Value { get; set; }
        public ulong ServerId { get; set; }
        public string Name { get; }
        public abstract T DefaultValue { get; }

        public MappedConfiguration()
        {
            this.Value = this.DefaultValue;
            this.Name = this.GetType().Name;
        }
    }
}
