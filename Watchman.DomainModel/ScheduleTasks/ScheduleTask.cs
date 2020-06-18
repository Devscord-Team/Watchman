using System;
using System.Collections.Generic;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.ScheduleTasks
{
    public class ScheduleTask : Entity
    {
        public string CommandName { get; set; }
        public IEnumerable<object> Arguments { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool IsExecuted { get; set; }

        public ScheduleTask()
        {
        }

        public ScheduleTask(string commandName, IEnumerable<object> arguments, DateTime executionDate)
        {
            this.CommandName = commandName;
            this.Arguments = arguments;
            this.ExecutionDate = executionDate;
        }

        public void SetAsExecuted() => this.IsExecuted = true;
    }
}
