using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
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
            throw new NotImplementedException();
        }
    }
}
