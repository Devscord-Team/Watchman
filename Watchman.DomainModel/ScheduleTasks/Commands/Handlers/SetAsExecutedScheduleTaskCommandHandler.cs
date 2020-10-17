using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Integrations.Database;

namespace Watchman.DomainModel.ScheduleTasks.Commands.Handlers
{
    public class SetAsExecutedScheduleTaskCommandHandler : ICommandHandler<SetAsExecutedScheduleTaskCommand>
    {
        private readonly ISessionFactory _sessionFactory;

        public SetAsExecutedScheduleTaskCommandHandler(ISessionFactory sessionFactory)
        {
            this._sessionFactory = sessionFactory;
        }

        public async Task HandleAsync(SetAsExecutedScheduleTaskCommand command)
        {
            using var session = this._sessionFactory.CreateMongo();
            var scheduleTask = session.Get<ScheduleTask>(command.ScheduleTaskId);
            scheduleTask.SetAsExecuted();
            await session.UpdateAsync(scheduleTask);
        }
    }
}
