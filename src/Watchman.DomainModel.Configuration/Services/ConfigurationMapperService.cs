using System;
using System.Collections.Generic;
using System.Linq;

namespace Watchman.DomainModel.Configuration.Services
{
    public class ConfigurationMapperService
    {
        private readonly ConfigurationItemsSearcherService _configurationItemsSearcher;

        public ConfigurationMapperService(ConfigurationItemsSearcherService configurationItemsSearcher)
        {
            this._configurationItemsSearcher = configurationItemsSearcher;
        }

        public Dictionary<Type, Dictionary<ulong, IMappedConfiguration>> GetMappedConfigurations(IEnumerable<ConfigurationItem> configurationItems)
        {
            var groupedByTypes = configurationItems.GroupBy(x => x.Name);
            return groupedByTypes.Select(this.MakeServersDictionary).ToDictionary(x => x.Values.First().GetType(), x => x);
        }

        public ConfigurationItem MapIntoBaseFormat(IMappedConfiguration mappedConfiguration)
        {
            return new ConfigurationItem(((dynamic) mappedConfiguration).Value, mappedConfiguration.ServerId, mappedConfiguration.Name);
        }

        private Dictionary<ulong, IMappedConfiguration> MakeServersDictionary(IEnumerable<ConfigurationItem> configurationItems)
        {
            return configurationItems.ToDictionary(x => x.ServerId, x => this.MapConfiguration(x));
        }

        private IMappedConfiguration MapConfiguration(ConfigurationItem configurationItem)
        {
            var type = this._configurationItemsSearcher.ConfigurationTypes.First(x => x.Name == configurationItem.Name);
            dynamic mappedConfiguration = Activator.CreateInstance(type, configurationItem.ServerId);
            mappedConfiguration!.Value = (dynamic) configurationItem.Value;
            return mappedConfiguration;
        }
    }
}
