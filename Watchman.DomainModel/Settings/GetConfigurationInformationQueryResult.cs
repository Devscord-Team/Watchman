using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings
{
    public class GetConfigurationInformationQueryResult : IQueryResult
    {
        public IEnumerable<ConfigurationItem> ConfigurationInformation { get; }

        public GetConfigurationInformationQueryResult(IEnumerable<ConfigurationItem> configurationInformation)
        {
            this.ConfigurationInformation = configurationInformation;
        }
    }
}