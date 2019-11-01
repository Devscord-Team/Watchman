using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Discord.Areas.Commons.Models;
using Watchman.Discord.Areas.Statistics.Models;

namespace Watchman.Discord.Areas.Statistics.Services
{
    public class ReportsService
    {
        //TODO unit test
        public StatisticsReport CreateReport(IEnumerable<MessageInformation> messages, Period period)
        {
            if (!messages.Any())
            {
                return default;
            }

            var sortedMessages = messages.OrderByDescending(x => x.Date);
            var latestDateBasedOnPeriod = this.GetLatestDateBasedOnPeriod(messages.First().Date, period);

            var statisticsPerPeriod = this.SplitMessagesToReportsPerPeriod(messages, latestDateBasedOnPeriod, period);

            return new StatisticsReport
            {
                AllMessages = statisticsPerPeriod.Sum(x => x.MessagesQuantity),
                StatisticsPerPeriod = statisticsPerPeriod,
                TimeRange = new TimeRange { Start = statisticsPerPeriod.First().TimeRange.Start, End = latestDateBasedOnPeriod }
            };
        }

        private IEnumerable<StatisticsReportPeriod> SplitMessagesToReportsPerPeriod(IEnumerable<MessageInformation> messages, DateTime latestDate, Period period)
        {
            var result = new List<StatisticsReportPeriod>();

            var currentPeriod = new TimeRange { Start = this.GetOldestMessageInCurrentPeriod(latestDate, period), End = latestDate };
            var messagesInCurrentPeriod = new List<MessageInformation>();
            do
            {
                messagesInCurrentPeriod = messages.Where(x => x.Date >= currentPeriod.Start && x.Date <= currentPeriod.End).ToList();
                var statisticsInCurrentPeriod = new StatisticsReportPeriod
                {
                    MessagesQuantity = messagesInCurrentPeriod.Count,
                    Period = period,
                    TimeRange = currentPeriod
                };
                result.Add(statisticsInCurrentPeriod);
                currentPeriod = this.TransferToPreviousPeriod(currentPeriod, period);

            } while (messagesInCurrentPeriod.Any());
            
            return result;
        }

        private TimeRange TransferToPreviousPeriod(TimeRange currentPeriod, Period period)
        {
            switch (period)
            {
                case Period.Hour:
                    return new TimeRange
                    {
                        Start = currentPeriod.Start.AddHours(-1),
                        End = currentPeriod.End.AddHours(-1)
                    };
                case Period.Day:
                    return new TimeRange
                    {
                        Start = currentPeriod.Start.AddDays(-1),
                        End = currentPeriod.End.AddDays(-1),
                    };
                case Period.Week:
                    return new TimeRange
                    {
                        Start = currentPeriod.Start.AddDays(-7),
                        End = currentPeriod.End.AddDays(-7),
                    };
                case Period.Month:
                    return new TimeRange
                    {
                        Start = currentPeriod.Start.AddMonths(-1),
                        End = currentPeriod.End.AddMonths(-1),
                    };
            }
            return default;
        }

        private DateTime GetOldestMessageInCurrentPeriod(DateTime startOfPeriod, Period period)
        {
            switch (period)
            {
                case Period.Hour:
                    return startOfPeriod.AddHours(-1);
                case Period.Day:
                    return startOfPeriod.AddDays(-1);
                case Period.Week:
                    return startOfPeriod.AddDays(-7);
                case Period.Month:
                    return startOfPeriod.AddMonths(-1);
            }
            return default;
        }

        private DateTime GetLatestDateBasedOnPeriod(DateTime latestDate, Period period)
        {
            switch (period)
            {
                case Period.Hour:
                    return new DateTime(latestDate.Year, latestDate.Month, latestDate.Day, latestDate.Hour + 1, 0, 0);
                case Period.Day:
                    return new DateTime(latestDate.Year, latestDate.Month, latestDate.Day);
                case Period.Week:
                    return new DateTime(latestDate.Year, latestDate.Month, latestDate.Day).AddDays(-(int)latestDate.DayOfWeek + 6); //should be sunday
                case Period.Month:
                    return new DateTime(latestDate.Year, latestDate.Month + 1, 1).AddDays(-1); //last day of current month
            }
            return default;
        }
    }
}
