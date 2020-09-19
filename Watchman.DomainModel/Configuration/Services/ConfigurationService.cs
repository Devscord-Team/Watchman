using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Configuration.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private const ulong DEFAULT_SERVER_ID = 0;
        private readonly ISessionFactory _sessionFactory;
        private readonly ConfigurationMapperService _configurationMapperService;
        private readonly ConfigurationItemsSearcherService _configurationTypesSearcher;
        private readonly List<(string configurationName, ulong serverId, Func<IMappedConfiguration, Task> func)> _afterConfigurationChanged = new List<(string, ulong, Func<IMappedConfiguration, Task>)>();
        private Dictionary<Type, Dictionary<ulong, IMappedConfiguration>> _cachedConfigurationItem;

        public ConfigurationService(ISessionFactory sessionFactory, ConfigurationMapperService configurationMapperService, ConfigurationItemsSearcherService configurationTypesSearcher)
        {
            this._sessionFactory = sessionFactory;
            this._configurationMapperService = configurationMapperService;
            this._configurationTypesSearcher = configurationTypesSearcher;
            this.Refresh();
        }

        public T GetConfigurationItem<T>(ulong serverId) where T : IMappedConfiguration
        {
            var configurations = this._cachedConfigurationItem[typeof(T)];
            var serverConfiguration = configurations.GetValueOrDefault(serverId) ?? configurations[0];
            return (T)serverConfiguration;
        }

        public void AddOnConfigurationChanged(string configurationName, ulong serverId, Func<IMappedConfiguration, Task> func)
        {
            this._afterConfigurationChanged.Add((configurationName, serverId, func));
        }

        public IEnumerable<IMappedConfiguration> GetConfigurationItems(ulong serverId)
        {
            return this._cachedConfigurationItem.Select(x => x.Value.GetValueOrDefault(serverId) ?? x.Value[DEFAULT_SERVER_ID]);
        }

        public async Task SaveNewConfiguration(IMappedConfiguration changedConfiguration)
        {
            using var session = this._sessionFactory.Create();
            var existingConfiguration = session.Get<ConfigurationItem>()
                .FirstOrDefault(x => x.ServerId == changedConfiguration.ServerId && x.Name == changedConfiguration.Name);
            var baseFormatConfigurationItem = this._configurationMapperService.MapIntoBaseFormat(changedConfiguration);
            if (existingConfiguration == null)
            {
                await session.AddAsync(baseFormatConfigurationItem);
            }
            else
            {
                existingConfiguration.SetValue(baseFormatConfigurationItem.Value);
                await session.AddOrUpdateAsync(existingConfiguration);
            }
            this.Refresh();
        }

        public async Task InitDefaultConfigurations()
        {
            var configurationsTypes = this._configurationTypesSearcher.ConfigurationTypes;
            var configurations = configurationsTypes.Select(x =>
            {
                var conf = (IMappedConfiguration)Activator.CreateInstance(x, DEFAULT_SERVER_ID);
                return this._configurationMapperService.MapIntoBaseFormat(conf);
            });
            using var session = this._sessionFactory.Create();
            var existingConfigurations = session.Get<ConfigurationItem>().Where(x => x.ServerId == 0).ToList();
            foreach (var configuration in configurations.Where(x => existingConfigurations.All(y => x.Name != y.Name)))
            {
                await session.AddAsync(configuration);
            }
            this.Refresh();
        }

        public void Refresh()
        {
            using var session = this._sessionFactory.Create();
            var configurationItems = session.Get<ConfigurationItem>();
            var changedConfiguration = configurationItems.Where(x => x.Version)
            var mappedConfigurations = this._configurationMapperService.GetMappedConfigurations(configurationItems);
            var changedConfigurations = mappedConfigurations.SelectMany(x =>
            {
                var (configurationType, serversConfigurations) = x;
                return serversConfigurations.Where(configuration =>
                {
                    var (serverId, mappedConfiguration) = configuration;
                    var existingConfiguration = this._cachedConfigurationItem[configurationType].GetValueOrDefault(serverId);
                    return !mappedConfiguration.Equals(existingConfiguration);
                });
            });
            var afterConfigurationChangedTasks = new List<Task>();
            foreach (var (serverId, configuration) in changedConfigurations)
            {
                var funcsToCall = this._afterConfigurationChanged.Where(x => x.serverId == serverId && x.configurationName == configuration.Name);
                var tasks = funcsToCall.Select(x => x.func!.Invoke(configuration));
                afterConfigurationChangedTasks.AddRange(tasks);
            }
            this._cachedConfigurationItem = mappedConfigurations;
            Task.WaitAll(afterConfigurationChangedTasks.ToArray());
        }
    }
}
