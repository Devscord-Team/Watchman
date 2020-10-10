using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Autofac;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Hangfire;
using Statsman.Core.Generators;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.DomainModel.Messages;
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
                (container.Resolve<UnmutingService>(), RefreshFrequent.Quarterly, true) // if RefreshFrequent changed remember to change SHORT_MUTE_TIME_IN_MINUTES in unmutingService!
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
            var configurationService = container.Resolve<ConfigurationService>();
            recurringJobManager.AddOrUpdate(nameof(ConfigurationService), () => configurationService.Refresh(), this.GetCronExpression(RefreshFrequent.Minutely));

            var discordServersService = container.Resolve<DiscordServersService>();
            var statisticsGenerator = container.Resolve<PreStatisticsGenerator>();
            recurringJobManager.AddOrUpdate(nameof(PreStatisticsGenerator), 
                () => GenerateStatistics(discordServersService, statisticsGenerator, Period.Day), 
                this.GetCronExpression(RefreshFrequent.Daily));
            recurringJobManager.AddOrUpdate(nameof(PreStatisticsGenerator),
                () => GenerateStatistics(discordServersService, statisticsGenerator, Period.Month),
                this.GetCronExpression(RefreshFrequent.Weekly));
            recurringJobManager.AddOrUpdate(nameof(PreStatisticsGenerator),
                () => GenerateStatistics(discordServersService, statisticsGenerator, Period.Quarter),
                this.GetCronExpression(RefreshFrequent.Monthly));


        }

        private void GenerateStatistics(DiscordServersService discordServersService, PreStatisticsGenerator statisticsGenerator, string period)
        {
            var serverIds = discordServersService.GetDiscordServersAsync().Select(x => x.Id).ToListAsync().Result;
            var tasks = period switch
            {
                Period.Day => serverIds.Select(x => statisticsGenerator.PreGenerateStatisticsPerDay(x)).ToArray(),
                Period.Month => serverIds.Select(x => statisticsGenerator.PreGenerateStatisticsPerDay(x)).ToArray(),
                Period.Quarter => serverIds.Select(x => statisticsGenerator.PreGenerateStatisticsPerDay(x)).ToArray(),
                _ => throw new NotImplementedException()
            };
            Task.WaitAll(tasks);
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
