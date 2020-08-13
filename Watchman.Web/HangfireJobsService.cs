using System;
using System.Collections.Generic;
using Autofac;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Hangfire;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.DomainModel.Settings.Services;

namespace Watchman.Web
{
    public class HangfireJobsService
    {
        public void SetDefaultJobs(IContainer container)
        {
            var generators = new List<(ICyclicService, RefreshFrequent, bool shouldTriggerNow)>
            {
                (container.Resolve<MessagesService>(), RefreshFrequent.Quarterly, true),
                (container.Resolve<ServerMessagesCacheService>(), RefreshFrequent.Quarterly, false),
                (container.Resolve<ResponsesCleanupService>(), RefreshFrequent.Daily, false),
                (container.Resolve<UnmutingService>(), RefreshFrequent.Quarterly, true), // if RefreshFrequent changed remember to change SHORT_MUTE_TIME_IN_MINUTES in unmutingService!
                (container.Resolve<CyclicStatisticsGeneratorService>(), RefreshFrequent.Daily, false),
                (container.Resolve<CheckUserSafetyService>(), RefreshFrequent.Daily, true)
            };
            var recurringJobManager = container.Resolve<IRecurringJobManager>();
            foreach (var (generator, refreshFrequent, shouldTrigger) in generators)
            {
                var cronExpression = this.GetCronExpression(refreshFrequent);
                recurringJobManager.AddOrUpdate(generator.GetType().Name, () => generator.Refresh(), cronExpression);
                if (shouldTrigger)
                {
                    recurringJobManager.Trigger(generator.GetType().Name);
                }
            }
            var service = container.Resolve<ConfigurationService>();
            recurringJobManager.AddOrUpdate(nameof(ConfigurationService), () => service.Refresh(), this.GetCronExpression(RefreshFrequent.Minutely));
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
