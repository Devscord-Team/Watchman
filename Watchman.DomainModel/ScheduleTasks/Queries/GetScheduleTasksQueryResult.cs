using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Tasks;

namespace Watchman.DomainModel.ScheduleTasks.Queries
{
    public class GetScheduleTasksQueryResult : IQueryResult
    {
        public IEnumerable<ScheduleTask> ScheduleTasks { get; private set; }

        public GetScheduleTasksQueryResult(IEnumerable<ScheduleTask> scheduleTasks)
        {
            this.ScheduleTasks = scheduleTasks;
        }
    }
}
