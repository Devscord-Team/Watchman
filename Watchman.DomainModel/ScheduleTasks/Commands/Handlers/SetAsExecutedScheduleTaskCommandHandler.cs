using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Tasks;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ScheduleTasks.Commands.Handlers
{
    public class SetAsExecutedScheduleTaskCommandHandler : ICommandHandler<SetAsExecutedScheduleTaskCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public SetAsExecutedScheduleTaskCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Task HandleAsync(SetAsExecutedScheduleTaskCommand command)
        {
            using (var session = sessionFactory.Create())
            {
                var scheduleTask = session.Get<ScheduleTask>(command.ScheduleTaskId);
                scheduleTask.SetAsExecuted();
                session.Update(scheduleTask);
            }
            return Task.CompletedTask;
        }
    }
}
