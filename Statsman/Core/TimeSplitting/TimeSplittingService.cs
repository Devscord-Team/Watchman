using Statsman.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Watchman.Common.Models;
using Watchman.DomainModel.Messages;

namespace Statsman.Core.TimeSplitting
{
    public class TimeSplittingService
    {
        public IEnumerable<TimeStatisticItem> GetStatisticsPerDay(IEnumerable<ServerDayStatistic> serverDayStatistics, IEnumerable<Message> latestMessages, TimeRange expectedTimeRange)
        {
            var latestPreGeneratedDate = serverDayStatistics.OrderBy(x => x.UpdatedAt).First();
            latestMessages = latestMessages.Where(x => 
            {
                if (x.SentAt.Date > latestPreGeneratedDate.Date) //in newer day
                {
                    return true;
                }
                if (x.SentAt > latestPreGeneratedDate.CreatedAt) //in same day, but later
                {
                    return true;
                }
                return false;
            }).ToList();
            var oldestLastMessagesDate = latestMessages.OrderBy(x => x.SentAt).First().SentAt;

            var result = new List<TimeStatisticItem>();
            expectedTimeRange.ForeachDay((i, day) => 
            {
                var sum = 0;
                if(day >= oldestLastMessagesDate.Date)
                {
                    sum += latestMessages.Where(x => x.SentAt.Date == day).Count();
                }
                sum += serverDayStatistics.Where(x => x.Date.Date == day).OrderBy(x => x.CreatedAt).FirstOrDefault()?.Count ?? 0;
                var item = new TimeStatisticItem(TimeRange.Create(day, day.AddDays(1).AddSeconds(-1)), sum);
                result.Add(item);
            });
            return result;
        }
    }
}
