using Watchman.Cqrs;

namespace Watchman.DomainModel.Configuration.Queries
{
    public class GetConfigurationItemsQuery : IQuery<GetConfigurationItemsQueryResult>
    {
        public string Group { get; private set; }

        public GetConfigurationItemsQuery(string group = null)
        {
            this.Group = group;
        }
    }
}
