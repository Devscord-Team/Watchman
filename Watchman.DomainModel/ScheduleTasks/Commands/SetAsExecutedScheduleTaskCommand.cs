using System;
using System.Collections.Generic;
using System.Text;
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
