using System;
using System.Collections.Generic;
using Autofac;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Hangfire;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Statistics.Services;

namespace Watchman.Web
{
    public class HangfireJobsService
    {
        public void SetDefaultJobs(IContainer container)
        {
            var generators = new Dictionary<ICyclicService, RefreshFrequent>
            {
                {container.Resolve<CyclicStatisticsGeneratorService>(), RefreshFrequent.Daily},
                {container.Resolve<CheckUserSafetyStrategyService>(), RefreshFrequent.Daily},
                {container.Resolve<UnmutingService>(), RefreshFrequent.Quarterly},
                {container.Resolve<MessagesService>(), RefreshFrequent.Quarterly}
            };
            var recurringJobManager = container.Resolve<IRecurringJobManager>();
            foreach (var (generator, refreshFrequent) in generators)
            {
                var cronExpression = this.GetCronExpression(refreshFrequent);
                recurringJobManager.AddOrUpdate(generator.GetType().Name, () => generator.Refresh(), cronExpression);
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
