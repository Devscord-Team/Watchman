using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Settings.ConfigurationItems;

namespace Watchman.DomainModel.Settings.Services
{
    public class ConfigurationItemsSearcherService
    {
        public List<Type> GetConfigurationTypes() => new List<Type>
        {
            typeof(MinAverageMessagesPerWeek),
            typeof(PercentOfSimilarityBetweenMessagesToSuspectSpam)
        };
    }

    public class ConfigurationMapperService
    {
        private readonly ConfigurationItemsSearcherService _configurationItemsSearcher;

        public ConfigurationMapperService(ConfigurationItemsSearcherService configurationItemsSearcher)
        {
            this._configurationItemsSearcher = configurationItemsSearcher;
        }

        public Dictionary<Type, Dictionary<ulong, IMappedConfiguration>> GetMappedConfigurations(IEnumerable<ConfigurationItem> configurationItems)
        {
            var groupedByTypes = configurationItems.GroupBy(x => x.ConfigurationName);
            return groupedByTypes.Select(x => MakeServersDict(x)).ToDictionary(x => x.Values.First().GetType(), x => x);
        }

        public ConfigurationItem MapIntoBaseFormat(IMappedConfiguration mappedConfiguration)
        {
            return new ConfigurationItem
            {
                ServerId = ((dynamic)mappedConfiguration).ServerId,
                ConfigurationName = mappedConfiguration.GetType().Name,
                Value = ((dynamic)mappedConfiguration).Value
            };
        }

        private Dictionary<ulong, IMappedConfiguration> MakeServersDict(IEnumerable<ConfigurationItem> configurationItems)
        {
            return configurationItems.ToDictionary(x => x.ServerId, x => this.MapConfiguration(x));
        }

        private IMappedConfiguration MapConfiguration(ConfigurationItem configurationItem)
        {
            var type = this._configurationItemsSearcher.GetConfigurationTypes().First(x => x.Name == configurationItem.ConfigurationName);
            return (IMappedConfiguration)Activator.CreateInstance(type);
        }
    }
}
