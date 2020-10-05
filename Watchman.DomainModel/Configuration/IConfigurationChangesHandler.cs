using System.Threading.Tasks;

namespace Watchman.DomainModel.Configuration
{
    public interface IConfigurationChangesHandler<in T> where T : IMappedConfiguration
    {
        Task Handle(ulong serverId, T newConfiguration);
    }
}
