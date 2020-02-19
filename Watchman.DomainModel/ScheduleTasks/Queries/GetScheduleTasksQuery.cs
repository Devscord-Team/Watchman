using Watchman.Cqrs;

namespace Watchman.DomainModel.ScheduleTasks.Queries
{
    public class GetScheduleTasksQuery : IQuery<GetScheduleTasksQueryResult>
    {
        public bool LoadOnlyActive { get; private set; }

        public GetScheduleTasksQuery(bool loadOnlyActive)
        {
            this.LoadOnlyActive = loadOnlyActive;
        }
    }
}
