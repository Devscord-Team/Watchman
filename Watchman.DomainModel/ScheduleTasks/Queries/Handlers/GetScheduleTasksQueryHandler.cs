using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Tasks;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ScheduleTasks.Queries.Handlers
{
    public class GetScheduleTasksQueryHandler : IQueryHandler<GetScheduleTasksQuery, GetScheduleTasksQueryResult>
    {
        private readonly ISessionFactory sessionFactory;

        public GetScheduleTasksQueryHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public GetScheduleTasksQueryResult Handle(GetScheduleTasksQuery query)
        {
            using (var session = sessionFactory.Create())
            {
                var results = session.Get<ScheduleTask>();
                if(query.LoadOnlyActive)
                {
                    results = results.Where(x => !x.IsExecuted);
                }
                return new GetScheduleTasksQueryResult(results);
            }
        }
    }
}
