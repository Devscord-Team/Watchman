using System;
using Watchman.Cqrs;

namespace Watchman.DomainModel.ScheduleTasks.Commands
{
    public class SetAsExecutedScheduleTaskCommand : ICommand
    {
        public Guid ScheduleTaskId { get; private set; }

        public SetAsExecutedScheduleTaskCommand(Guid scheduleTaskId)
        {
            this.ScheduleTaskId = scheduleTaskId;
        }
    }
}
