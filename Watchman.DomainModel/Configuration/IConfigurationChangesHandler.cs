using System.Threading.Tasks;

namespace Watchman.DomainModel.Configuration
{
    public interface IConfigurationChangesHandler
    {
    }

    public interface IConfigurationChangesHandler<T> : IConfigurationChangesHandler where T : IMappedConfiguration
    {
        Task Handle(ulong serverId, T newConfiguration);
    }
}
