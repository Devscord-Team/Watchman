using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.CustomCommands.Queries.Handlers
{
    public class GetCustomCommandsQueryHandler : IQueryHandler<GetCustomCommandsQuery, GetCustomCommandsQueryResult>
    {
        private readonly ISessionFactory sessionFactory;

        public GetCustomCommandsQueryHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public GetCustomCommandsQueryResult Handle(GetCustomCommandsQuery query)
        {
            using var session = this.sessionFactory.CreateMongo();
            var customCommands = session.Get<CustomCommand>();
            if(query.ServerId > 0)
            {
                customCommands = customCommands.Where(x => x.ServerId == query.ServerId);
            }
            return new GetCustomCommandsQueryResult(customCommands);
        }
    }
}
