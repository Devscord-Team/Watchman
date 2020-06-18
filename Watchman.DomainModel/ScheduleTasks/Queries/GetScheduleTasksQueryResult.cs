using System.Collections.Generic;
using Watchman.Cqrs;

namespace Watchman.DomainModel.ScheduleTasks.Queries
{
    public class GetScheduleTasksQueryResult : IQueryResult
    {
        public IEnumerable<ScheduleTask> ScheduleTasks { get; private set; }

        public GetScheduleTasksQueryResult(IEnumerable<ScheduleTask> scheduleTasks) => this.ScheduleTasks = scheduleTasks;
    }
}
