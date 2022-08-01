using Autofac;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Models;
using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Watchman.DomainModel.Configuration.Services;
using Watchman.Web.Jobs;

namespace Watchman.Web
{
    public interface IHangfireJobsService
    {
        void AddJobs(IContainer container, IRecurringJobManager recurringJobManager);
        void SetDefaultJobs(IContainer container);
    }

    public class HangfireJobsService : IHangfireJobsService
    {
        public void SetDefaultJobs(IContainer container)
        {
            var recurringJobManager = container.Resolve<IRecurringJobManager>();
            this.AddJobs(container, recurringJobManager);
        }

        public void AddJobs(IContainer container, IRecurringJobManager recurringJobManager)
        {
            Assembly.GetAssembly(typeof(HangfireJobsService)).GetTypes()
                .Where(x => x.IsAssignableTo<IHangfireJob>() && !x.IsInterface)
                .Select(x => (x.Name, Job: (IHangfireJob)container.Resolve(x)))
                .ToList()
                .ForEach(x =>
                {
                    var jobName = this.FixJobName(x.Name);
                    recurringJobManager.AddOrUpdate(jobName, () => x.Job.Do(), this.GetCronExpression(x.Job.Frequency));
                    if (x.Job.RunOnStart)
                    {
                        recurringJobManager.Trigger(jobName);
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
