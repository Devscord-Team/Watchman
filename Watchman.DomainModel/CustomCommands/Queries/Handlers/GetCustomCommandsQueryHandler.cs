using Watchman.Cqrs;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.CustomCommands.Queries.Handlers
{
    public class GetCustomCommandsQueryHandler : IQueryHandler<GetCustomCommandsQuery, GetCustomCommandsQueryResult>
    {
        private readonly ISessionFactory sessionFactory;

        public GetCustomCommandsQueryHandler(ISessionFactory sessionFactory) => this.sessionFactory = sessionFactory;

        public GetCustomCommandsQueryResult Handle(GetCustomCommandsQuery query)
        {
            using var session = this.sessionFactory.Create();
            var customCommands = session.Get<CustomCommand>();
            return new GetCustomCommandsQueryResult(customCommands);
        }
    }
}
