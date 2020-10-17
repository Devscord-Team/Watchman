using System.Linq;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.ScheduleTasks.Queries.Handlers
{
    public class GetScheduleTasksQueryHandler : IQueryHandler<GetScheduleTasksQuery, GetScheduleTasksQueryResult>
    {
        private readonly ISessionFactory _sessionFactory;

        public GetScheduleTasksQueryHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public GetScheduleTasksQueryResult Handle(GetScheduleTasksQuery query)
        {
            using var session = this._sessionFactory.CreateMongo();
            var results = session.Get<ScheduleTask>();
            if (query.LoadOnlyActive)
            {
                results = results.Where(x => !x.IsExecuted);
            }
            return new GetScheduleTasksQueryResult(results.ToList());
        }
    }
}
