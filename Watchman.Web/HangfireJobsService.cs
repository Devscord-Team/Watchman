using System;
using System.Collections.Generic;
using Autofac;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Hangfire;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Statistics.Services;

namespace Watchman.Web
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
                var cronExpression = this.GetCronExpression(generator.RefreshFrequent);
                recurringJobManager.AddOrUpdate(generator.GetType().Name, () => generator.ReloadCache(), cronExpression);
            }
        }

        private string GetCronExpression(RefreshFrequent refreshFrequent)
        {
            return refreshFrequent switch
            {
                RefreshFrequent.Minutely => Cron.Minutely(),
                RefreshFrequent.Quarterly => "*/15 * * * *",
                RefreshFrequent.Hourly => Cron.Hourly(),
                RefreshFrequent.Daily => Cron.Daily(2),
                RefreshFrequent.Weekly => Cron.Weekly(),
                RefreshFrequent.Monthly => Cron.Monthly(),
                RefreshFrequent.Yearly => Cron.Yearly(),
                _ => throw new NotImplementedException()
            };
        }
    }
}
