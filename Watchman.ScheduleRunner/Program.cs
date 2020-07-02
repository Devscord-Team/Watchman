using Autofac;
using Watchman.Integrations.MongoDB;
using Watchman.ScheduleRunner.IoC;

namespace Watchman.ScheduleRunner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MongoConfiguration.Initialize();
            var autofac = (IComponentContext)new ContainerModule().GetBuilder().Build();

            var schedulerService = autofac.Resolve<SchedulerService>();
            schedulerService.RunScheduledTasks();
        }
    }
}
