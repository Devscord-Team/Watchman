using System;
using System.Collections.Generic;
using System.Text;
using Watchman.Cqrs;

namespace Watchman.DomainModel.ScheduleTasks.Commands
{
    public class AddScheduleTaskCommand : ICommand
    {
        public string CommandName { get; }
        public IEnumerable<object> Arguments { get; }
        public DateTime ExecutionDate { get; }

        public AddScheduleTaskCommand(string commandName, IEnumerable<object> arguments, DateTime executionDate)
        {
            CommandName = commandName;
            Arguments = arguments;
            ExecutionDate = executionDate;
        }
    }
}
