namespace Watchman.DomainModel.Settings
{
    public interface IMappedConfiguration
    {
        ulong ServerId { get; }
        string Name { get; }
    }
}
