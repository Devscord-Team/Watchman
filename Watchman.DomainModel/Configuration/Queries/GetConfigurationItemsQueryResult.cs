using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Configuration.Queries
{
    public class GetConfigurationItemsQueryResult : IQueryResult
    {
        public IEnumerable<ConfigurationItem> ConfigurationItems { get; private set; }

        public GetConfigurationItemsQueryResult(IEnumerable<ConfigurationItem> configurationItems)
        {
            this.ConfigurationItems = configurationItems;
        }
    }
}
