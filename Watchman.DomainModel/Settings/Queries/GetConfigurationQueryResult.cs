using Watchman.Cqrs;

namespace Watchman.DomainModel.Settings.Queries
{
    public class GetConfigurationQueryResult : IQueryResult
    {
        public Configuration Configuration { get; private set; }
        
        public GetConfigurationQueryResult(Configuration configuration)
        {
            this.Configuration = configuration;
        }
    }
}
