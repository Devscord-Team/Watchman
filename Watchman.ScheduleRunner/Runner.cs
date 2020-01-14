using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.DomainModel.Tasks;

namespace Watchman.ScheduleRunner
{
    public class Runner
    {
        private readonly ICommandBus commandBus;
        private readonly Assembly assembly;

        public Runner(ICommandBus commandBus, Assembly assembly)
        {
            this.commandBus = commandBus;
            this.assembly = assembly;
        }

        public Task Run(ScheduleTask scheduleTask)
        {
            var commandType = this.assembly.GetTypes().FirstOrDefault(x => x.FullName == scheduleTask.CommandName);
            if(scheduleTask == null)
            {
                throw new ArgumentException($"Not found {scheduleTask.CommandName} command");
            }
            var instance = (ICommand) Activator.CreateInstance(commandType, scheduleTask.Arguments.ToArray());
            return this.commandBus.ExecuteAsync(instance);
        }

    }
}
