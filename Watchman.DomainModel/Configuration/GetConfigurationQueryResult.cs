using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Configuration
{
    public class GetConfigurationQueryResult : IQueryResult
    {
        public IEnumerable<ConfigurationItem> ConfigurationItems { get; }

        public GetConfigurationQueryResult(IEnumerable<ConfigurationItem> configurationItems)
        {
            this.ConfigurationItems = configurationItems;
        }
    }
}