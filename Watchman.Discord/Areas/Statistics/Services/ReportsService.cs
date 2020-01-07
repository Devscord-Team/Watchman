using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Statistics.Models;

namespace Watchman.Discord.Areas.Statistics.Services
{
    public class ReportsService
    {
        public Period SelectPeriod(string period)
        {
            if (period.ToLowerInvariant().Contains("hour"))
            {
               return Period.Hour;
            }
            else if (period.ToLowerInvariant().Contains("day"))
            {
                return Period.Day;
            }
            else if (period.ToLowerInvariant().Contains("week"))
            {
                return Period.Week;
            }
            else if (period.ToLowerInvariant().Contains("month"))
            {
                return Period.Month;
            }
            return Period.Day;
        }


        //TODO unit test
        public StatisticsReport CreateReport(IEnumerable<MessageInformation> messages, Period period, DiscordServerContext serverContext)
        {
            var serverMessages = messages.Where(x => x.Server.Id == serverContext.Id).ToList();

            if (!serverMessages.Any())
            {
                return default;
            }

            var sortedMessages = serverMessages.OrderByDescending(x => x.SentAt).ToList();
            var latestDateBasedOnPeriod = this.GetLatestDateBasedOnPeriod(sortedMessages.First().SentAt, period);

            var statisticsPerPeriod = this.SplitMessagesToReportsPerPeriod(sortedMessages, latestDateBasedOnPeriod, period);

            return new StatisticsReport
            {
                AllMessages = statisticsPerPeriod.Sum(x => x.MessagesQuantity),
                StatisticsPerPeriod = statisticsPerPeriod,
                TimeRange = new TimeRange { Start = statisticsPerPeriod.First().TimeRange.Start, End = latestDateBasedOnPeriod }
            };
        }

        private IEnumerable<StatisticsReportPeriod> SplitMessagesToReportsPerPeriod(List<MessageInformation> messages, DateTime latestDate, Period period)
        {
            var result = new List<StatisticsReportPeriod>();
            var lastMessageDate = messages.Last().SentAt.Date;

            var currentPeriod = new TimeRange { Start = this.GetOldestMessageInCurrentPeriod(latestDate, period), End = latestDate };
            do
            {
                var messagesInCurrentPeriod = messages.Where(x => x.SentAt >= currentPeriod.Start && x.SentAt <= currentPeriod.End);
                var statisticsInCurrentPeriod = new StatisticsReportPeriod
                {
                    MessagesQuantity = messagesInCurrentPeriod.Count(),
                    Period = period,
                    TimeRange = currentPeriod
                };
                result.Add(statisticsInCurrentPeriod);
                currentPeriod = this.TransferToPreviousPeriod(currentPeriod, period);

            } while (currentPeriod.Start.Date >= lastMessageDate);
            return result;
        }

        private TimeRange TransferToPreviousPeriod(TimeRange currentPeriod, Period period)
        {
            return new TimeRange
            {
                Start = this.GetOldestMessageInCurrentPeriod(currentPeriod.Start.AddMinutes(-1), period),
                End = this.GetLatestDateBasedOnPeriod(currentPeriod.Start.AddMinutes(-1), period),
            };
        }

        private DateTime GetOldestMessageInCurrentPeriod(DateTime endOfPeriod, Period period)
        {
            return period switch
            {
                Period.Hour => new DateTime(endOfPeriod.Year, endOfPeriod.Month, endOfPeriod.Day, endOfPeriod.Hour, 0, 0),
                Period.Day => new DateTime(endOfPeriod.Year, endOfPeriod.Month, endOfPeriod.Day),
                Period.Week => new DateTime(endOfPeriod.Year, endOfPeriod.Month, endOfPeriod.Day).AddDays(-6),
                Period.Month => new DateTime(endOfPeriod.Year, endOfPeriod.Month, 1),
                _ => default
            };
        }

        private DateTime GetLatestDateBasedOnPeriod(DateTime latestDate, Period period)
        {
            return period switch
            {
                Period.Hour => new DateTime(latestDate.Year, latestDate.Month, latestDate.Day, latestDate.Hour, 0, 0).AddHours(1).AddMilliseconds(-1),
                Period.Day => new DateTime(latestDate.Year, latestDate.Month, latestDate.Day).AddDays(1).AddMilliseconds(-1),
                Period.Week => new DateTime(latestDate.Year, latestDate.Month, latestDate.Day).AddDays(-(int) latestDate.DayOfWeek + 7).AddDays(1).AddMilliseconds(-1), //should be sunday
                Period.Month => new DateTime(latestDate.Year, latestDate.Month, DateTime.DaysInMonth(latestDate.Year, latestDate.Month)).AddDays(1).AddMilliseconds(-1), //last day of current month
            _ => default
            };
        }
    }
}
