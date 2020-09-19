using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Watchman.DomainModel.Configuration.Services
{
    public interface IConfigurationService
    {
        T GetConfigurationItem<T>(ulong serverId) where T : IMappedConfiguration;
        void AddOnConfigurationChanged(string configurationName, ulong serverId, Func<IMappedConfiguration, Task> func);
        IEnumerable<IMappedConfiguration> GetConfigurationItems(ulong serverId);
        Task SaveNewConfiguration(IMappedConfiguration changedConfiguration);
    }
}
