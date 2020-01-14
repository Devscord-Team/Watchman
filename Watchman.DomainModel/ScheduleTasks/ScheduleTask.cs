using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Tasks
{
    public class ScheduleTask : Entity
    {
        public string CommandName { get; set; }
        public IEnumerable<object> Arguments { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool IsExecuted { get; set; }
    }
}
