using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.ScheduleTasks.Queries;

namespace Watchman.ScheduleRunner
{
    public class SchedulerService
    {
        private readonly IQueryBus queryBus;
        private readonly ScheduleTasksRunner tasksRunner;

        public SchedulerService(IQueryBus queryBus, ScheduleTasksRunner tasksRunner)
        {
            this.queryBus = queryBus;
            this.tasksRunner = tasksRunner;
        }

        public void RunScheduledTasks()
        {
            var query = new GetScheduleTasksQuery(true);
            var scheduleTasks = this.queryBus.Execute(query).ScheduleTasks;
            Parallel.ForEach(scheduleTasks, x => this.tasksRunner.Run(x).Wait());
        }
    }
}
