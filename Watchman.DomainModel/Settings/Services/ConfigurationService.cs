using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Settings.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ISessionFactory _sessionFactory;
        private readonly ConfigurationMapperService _configurationMapperService;
        private readonly ConfigurationItemsSearcherService _configurationTypesSearcher;
        private Dictionary<Type, Dictionary<ulong, IMappedConfiguration>> _cachedConfigurationItem;
        private DateTime _lastRefreshed;

        public ConfigurationService(ISessionFactory sessionFactory, ConfigurationMapperService configurationMapperService, ConfigurationItemsSearcherService configurationTypesSearcher)
        {
            this._sessionFactory = sessionFactory;
            this._configurationMapperService = configurationMapperService;
            this._configurationTypesSearcher = configurationTypesSearcher;
        }

        public T GetConfigurationItem<T>(ulong serverId) where T : IMappedConfiguration
        {
            if (this._lastRefreshed < DateTime.Now.AddMinutes(-10))
            {
                this.ReloadConfiguration();
            }
            var configurations = this._cachedConfigurationItem[typeof(T)];
            var serverConfiguration = configurations.GetValueOrDefault(serverId) ?? configurations[0];
            return (T)serverConfiguration;
        }

        public IEnumerable<IMappedConfiguration> GetConfigurationItems(ulong serverId)
        {
            if (this._lastRefreshed < DateTime.Now.AddMinutes(-10))
            {
                this.ReloadConfiguration();
            }
            return this._cachedConfigurationItem.Select(x => x.Value.GetValueOrDefault(serverId) ?? x.Value[0]);
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
                existingConfiguration.Value = baseFormatConfigurationItem.Value;
                await session.AddOrUpdateAsync(existingConfiguration);
            }
        }

        public async Task InitDefaultConfigurations()
        {
            var configurationsTypes = this._configurationTypesSearcher.ConfigurationTypes;
            var configurations = configurationsTypes.Select(x =>
            {
                var conf = (IMappedConfiguration)Activator.CreateInstance(x);
                return this._configurationMapperService.MapIntoBaseFormat(conf);
            }).ToList();
            using (var session = this._sessionFactory.Create())
            {
                var existingConfigurations = session.Get<ConfigurationItem>().Where(x => x.ServerId == 0).ToList();

                foreach (var configuration in configurations.Where(x => existingConfigurations.All(y => x.Name != y.Name)))
                {
                    await session.AddAsync(configuration);
                }
            }
            this.ReloadConfiguration();
        }

        private void ReloadConfiguration()
        {
            using var session = this._sessionFactory.Create();
            var configurationItems = session.Get<ConfigurationItem>();
            this._cachedConfigurationItem = this._configurationMapperService.GetMappedConfigurations(configurationItems);
            this._lastRefreshed = DateTime.Now;
        }
    }
}
