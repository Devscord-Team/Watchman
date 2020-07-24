namespace Watchman.DomainModel.Settings.Services
{
    public interface IConfigurationService
    {
        T GetConfigurationItem<T>(ulong serverId) where T : IMappedConfiguration;
    }
}
