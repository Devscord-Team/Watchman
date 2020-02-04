using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Tasks;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ScheduleTasks.Commands.Handlers
{
    public class AddScheduleTaskCommandHandler : ICommandHandler<AddScheduleTaskCommand>
    {
        private readonly ISessionFactory sessionFactory;

        public AddScheduleTaskCommandHandler(ISessionFactory sessionFactory)
        {
            this.sessionFactory = sessionFactory;
        }

        public Task HandleAsync(AddScheduleTaskCommand command)
        {
            var scheduleTask = new ScheduleTask(command.CommandName, command.Arguments, command.ExecutionDate);
            using (var session = sessionFactory.Create())
            {
                session.Add(scheduleTask);
            }
            return Task.CompletedTask;
        }
    }
}
