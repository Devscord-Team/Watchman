using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.ScheduleTasks.Queries;

namespace Watchman.ScheduleRunner
{
    public class SchedulerService
    {
        private readonly IQueryBus _queryBus;
        private readonly ScheduleTasksRunner _tasksRunner;

        public SchedulerService(IQueryBus queryBus, ScheduleTasksRunner tasksRunner)
        {
            this._queryBus = queryBus;
            this._tasksRunner = tasksRunner;
        }

        public void RunScheduledTasks()
        {
            var query = new GetScheduleTasksQuery(loadOnlyActive: true);
            var scheduleTasks = this._queryBus.Execute(query).ScheduleTasks;
            Parallel.ForEach(scheduleTasks, x => this._tasksRunner.Run(x).Wait());
        }
    }
}
