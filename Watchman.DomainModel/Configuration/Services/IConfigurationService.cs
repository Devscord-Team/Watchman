namespace Watchman.DomainModel.Configuration.Services
{
    public interface IConfigurationService
    {
        T GetConfigurationItem<T>(ulong serverId) where T : IMappedConfiguration;
    }
}
