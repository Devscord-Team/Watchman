using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.ScheduleTasks;
using Watchman.DomainModel.ScheduleTasks.Commands;

namespace Watchman.ScheduleRunner
{
    public class ScheduleTasksRunner
    {
        private readonly ICommandBus _commandBus;
        private readonly Assembly _assembly;

        public ScheduleTasksRunner(ICommandBus commandBus, Assembly assembly)
        {
            this._commandBus = commandBus;
            this._assembly = assembly;
        }

        public async Task Run(ScheduleTask scheduleTask)
        {
            var commandType = this._assembly.GetTypes()
                .FirstOrDefault(x => x.FullName == scheduleTask.CommandName || x.Name == scheduleTask.CommandName);

            if(commandType == null)
            {
                throw new ArgumentException($"Not found {scheduleTask.CommandName} command");
            }

            var instance = (ICommand) Activator.CreateInstance(commandType, scheduleTask.Arguments.ToArray());
            await this._commandBus.ExecuteAsync(instance);
            var setAsExecutedCommand = new SetAsExecutedScheduleTaskCommand(scheduleTask.Id);
            await this._commandBus.ExecuteAsync(setAsExecutedCommand);
        }
    }
}
