using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.ScheduleTasks.Commands;
using Watchman.DomainModel.Tasks;

namespace Watchman.ScheduleRunner
{
    public class ScheduleTasksRunner
    {
        private readonly ICommandBus commandBus;
        private readonly Assembly assembly;

        public ScheduleTasksRunner(ICommandBus commandBus, Assembly assembly)
        {
            this.commandBus = commandBus;
            this.assembly = assembly;
        }

        public async Task Run(ScheduleTask scheduleTask)
        {
            var commandType = this.assembly.GetTypes()
                .FirstOrDefault(x => x.FullName == scheduleTask.CommandName || x.Name == scheduleTask.CommandName);
            if(scheduleTask == null)
            {
                throw new ArgumentException($"Not found {scheduleTask.CommandName} command");
            }
            var instance = (ICommand) Activator.CreateInstance(commandType, scheduleTask.Arguments.ToArray());
            await this.commandBus.ExecuteAsync(instance);
            var setAsExecutedComamand = new SetAsExecutedScheduleTaskCommand(scheduleTask.Id);
            await this.commandBus.ExecuteAsync(setAsExecutedComamand);
        }

    }
}
