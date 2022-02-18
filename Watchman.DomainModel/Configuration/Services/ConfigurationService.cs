using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.Configuration.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private const ulong DEFAULT_SERVER_ID = 0;
        private readonly IComponentContext _componentContext;
        private readonly ISessionFactory _sessionFactory;
        private readonly IConfigurationMapperService _configurationMapperService;
        private readonly IConfigurationItemsSearcherService _configurationTypesSearcher;
        private static Dictionary<Guid, int> _configurationVersions;
        private static Dictionary<Type, Dictionary<ulong, IMappedConfiguration>> _cachedConfigurationItem;

        public ConfigurationService(IComponentContext componentContext, ISessionFactory sessionFactory, 
            IConfigurationMapperService configurationMapperService, IConfigurationItemsSearcherService configurationTypesSearcher)
        {
            this._componentContext = componentContext;
            this._sessionFactory = sessionFactory;
            this._configurationMapperService = configurationMapperService;
            this._configurationTypesSearcher = configurationTypesSearcher;
            this.Refresh();
        }

        public T GetConfigurationItem<T>(ulong serverId) 
            where T : IMappedConfiguration
        {
            if(_cachedConfigurationItem.TryGetValue(typeof(T), out var configurations))
            {
                var configurationItem = configurations.GetValueOrDefault(serverId) ?? configurations[DEFAULT_SERVER_ID];
                return (T)configurationItem;
            }
            return (T) Activator.CreateInstance(typeof(T), new object[] { serverId });
        }

        public IEnumerable<IMappedConfiguration> GetConfigurationItems(ulong serverId)
        {
            return _cachedConfigurationItem.Select(x => x.Value.GetValueOrDefault(serverId) ?? x.Value[DEFAULT_SERVER_ID]);
        }

        public async Task SaveNewConfiguration(IMappedConfiguration changedConfiguration) // todo: saving new configuration not tested
        {
            using var session = this._sessionFactory.CreateMongo();
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
            using var session = this._sessionFactory.CreateMongo();
            var existingConfigurations = session.Get<ConfigurationItem>().Where(x => x.ServerId == 0).ToList();
            foreach (var configuration in configurations.Where(x => existingConfigurations.All(y => x.Name != y.Name)))
            {
                await session.AddAsync(configuration);
            }
            this.Refresh();
        }

        public void Refresh()
        {
            using var session = this._sessionFactory.CreateMongo();
            var configurationItems = session.Get<ConfigurationItem>().ToList();
            var mappedConfigurations = this._configurationMapperService.GetMappedConfigurations(configurationItems);
            if (_configurationVersions != null)
            {
                var changedConfigurationItems = configurationItems.Where(x => _configurationVersions.GetValueOrDefault(x.Id) != x.Version);
                var tasks = changedConfigurationItems.Select(changedConfiguration => // todo: this select wasn't tested
                { 
                    var sameTypeMappedConfigurations = mappedConfigurations.First(x => x.Key.Name == changedConfiguration.Name).Value;
                    var mappedConfiguration = sameTypeMappedConfigurations[changedConfiguration.ServerId];
                    var configurationChangesHandler = this.GetConfigurationChangesHandler(mappedConfiguration);
                    if (configurationChangesHandler == null)
                    {
                        throw new NotImplementedException($"configurationChangesHandler for {changedConfiguration.Name} is not implemented");
                    }
                    return configurationChangesHandler.Handle(changedConfiguration.ServerId, mappedConfiguration);
                });
                Task.WaitAll(tasks.ToArray());
            }
            _configurationVersions = configurationItems.ToDictionary(x => x.Id, x => x.Version);
            _cachedConfigurationItem = mappedConfigurations;
        }

        private IConfigurationChangesHandler<IMappedConfiguration> GetConfigurationChangesHandler(IMappedConfiguration newMappedConfiguration)
        {
            var configurationChangesHandlers = this._componentContext.ComponentRegistry.Registrations
                    .Where(x => typeof(IConfigurationChangesHandler).IsAssignableFrom(x.Activator.LimitType));
            var handlerForThisType = configurationChangesHandlers.FirstOrDefault(x => x.Activator.LimitType.Name.StartsWith(newMappedConfiguration.Name));
            if (handlerForThisType == null)
            {
                return null;
            }
            return this._componentContext.Resolve(handlerForThisType.Activator.LimitType) as IConfigurationChangesHandler<IMappedConfiguration>;
        }
    }
}
