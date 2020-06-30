using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Queries
{
    public class GetConfigurationQueryResult : IQueryResult
    {
        public ConfigurationItem ConfigurationItem { get; private set; }
        
        public GetConfigurationQueryResult(ConfigurationItem configurationItem)
        {
            this.ConfigurationItem = configurationItem;
        }
    }
}
