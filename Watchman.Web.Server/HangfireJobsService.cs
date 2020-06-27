using System.Collections.Generic;
using Autofac;
using Devscord.DiscordFramework.Services;
using Hangfire;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Statistics.Services;

namespace Watchman.Web.Server
{
    public class HangfireJobsService
    {
        public void SetDefaultJobs(IContainer container)
        {
            var generators = new List<ICyclicCacheGenerator>
            {
                container.Resolve<CyclicStatisticsGeneratorService>(),
                container.Resolve<CheckUserSafetyStrategyService>()
            };
            var recurringJobManager = container.Resolve<IRecurringJobManager>();
            foreach (var generator in generators)
            {
                recurringJobManager.AddOrUpdate(generator.GetType().Name, () => generator.ReloadCache(), Cron.Daily(2));
            }
        }
    }
}
