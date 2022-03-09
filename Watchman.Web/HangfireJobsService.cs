using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Hangfire;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.Web.Jobs;
using Watchman.DomainModel.Configuration.Services;

namespace Watchman.Web
{
    public interface IHangfireJobsService
    {
        void AddJobs(IContainer container, IRecurringJobManager recurringJobManager);
        void AddServices(IContainer container, IRecurringJobManager recurringJobManager);
        void SetDefaultJobs(IContainer container);
    }

    public class HangfireJobsService : IHangfireJobsService
    {
        public void SetDefaultJobs(IContainer container)
        {
            var recurringJobManager = container.Resolve<IRecurringJobManager>();
            this.AddServices(container, recurringJobManager);
            this.AddJobs(container, recurringJobManager);
        }

        public void AddServices(IContainer container, IRecurringJobManager recurringJobManager) //TODO maybe rewrite CyclicServices to Jobs would be good idea
        {
            var generators = new List<(ICyclicService, RefreshFrequent, bool shouldTriggerNow)>
            {
                (container.Resolve<ServerMessagesCacheService>(), RefreshFrequent.Quarterly, false),
                (container.Resolve<ResponsesCleanupService>(), RefreshFrequent.Daily, false),
                (container.Resolve<UnmutingService>(), RefreshFrequent.Quarterly, true) // if RefreshFrequent changed remember to change SHORT_MUTE_TIME_IN_MINUTES in unmutingService!
            };
            foreach (var (generator, refreshFrequent, shouldTrigger) in generators)
            {
                var cronExpression = this.GetCronExpression(refreshFrequent);
                recurringJobManager.AddOrUpdate(this.FixJobName(generator.GetType().Name), () => generator.Refresh(), cronExpression);
                if (shouldTrigger)
                {
                    recurringJobManager.Trigger(this.FixJobName(generator.GetType().Name));
                }
            }
            var configurationService = container.Resolve<ConfigurationService>();
            recurringJobManager.AddOrUpdate(nameof(ConfigurationService), () => configurationService.Refresh(), this.GetCronExpression(RefreshFrequent.Minutely));
        }

        public void AddJobs(IContainer container, IRecurringJobManager recurringJobManager)
        {
            Assembly.GetAssembly(typeof(HangfireJobsService)).GetTypes()
                .Where(x => x.IsAssignableTo<IHangfireJob>() && !x.IsInterface)
                .Select(x => (x.Name, Job: (IHangfireJob)container.Resolve(x))).ToList()
                .ForEach(x =>
                {
                    recurringJobManager.AddOrUpdate(this.FixJobName(x.Name), () => x.Job.Do(), this.GetCronExpression(x.Job.Frequency));
                    if (x.Job.RunOnStart)
                    {
                        recurringJobManager.Trigger(x.Name);
                    }
                });
        }

        private string FixJobName(string name)
        {
            var result = new List<char>();
            for (var i = 0; i < name.Length; i++)
            {
                var letter = name[i];
                if (i > 0 && letter == char.ToUpper(letter))
                {
                    result.Add(' ');
                }
                result.Add(letter);
            }
            return new string(result.ToArray());
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
