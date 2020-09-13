using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings
{
    public class GetConfigurationQueryResult : IQueryResult
    {
        public IEnumerable<ConfigurationItem> ConfigurationItems { get; }

        public GetConfigurationQueryResult(IEnumerable<ConfigurationItem> configurationItems)
        {
            ConfigurationItems = configurationItems;
        }
    }
}