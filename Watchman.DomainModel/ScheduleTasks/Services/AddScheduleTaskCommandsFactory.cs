using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.ScheduleTasks.Commands;
using Watchman.DomainModel.Tasks;

namespace Watchman.DomainModel.ScheduleTasks.Services
{
    public class AddScheduleTaskCommandsFactory
    {
        //TODO Add tests
        public AddScheduleTaskCommand Create(ICommand commandToExecute, DateTime executionDate)
        {
            var commandType = commandToExecute.GetType();
            var arguments = new List<object>();
            var constructorParameters = commandType.GetConstructors().First().GetParameters();
            foreach (var constructorParameter in constructorParameters)
            {
                var matchedProperty = commandType.GetProperties().First(x => x.Name.ToLower() == constructorParameter.Name.ToLower());
                var value = matchedProperty.GetValue(commandToExecute);
                arguments.Add(value);
            }
            return new AddScheduleTaskCommand(commandType.FullName, arguments, executionDate);
        }
    }
}
