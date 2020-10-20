using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Commands;
using Watchman.DomainModel.Messages.Queries;

namespace Statsman.Core.Generators
{
    public class PreGeneratedStatisticsGenerator
    {
        private List<PreGeneratedStatistic> preGeneratedStatistics = new List<PreGeneratedStatistic>();

        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;

        public PreGeneratedStatisticsGenerator(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
        }

        public Task PreGenerateStatisticsPerDay(ulong serverId) //todo test
        {
            return this.ProcessStatisticsPerPeriod(serverId, Period.Day);
        }

        public Task PreGenerateStatisticsPerMonth(ulong serverId) //todo test
        {
            return this.ProcessStatisticsPerPeriod(serverId, Period.Month);
        }

        public Task PreGenerateStatisticsPerQuarter(ulong serverId) //todo test
        {
            return this.ProcessStatisticsPerPeriod(serverId, Period.Quarter);
        }

        public Task ProcessStatisticsPerPeriod(ulong serverId, string period) //todo test
        {
            var messages = this.GetMessages(serverId);
            var oldestMessageDatetime = messages.OrderBy(x => x.SentAt).FirstOrDefault()?.SentAt ?? default;
            if (oldestMessageDatetime == default) //empty database
            {
                return Task.CompletedTask;
            }
            var users = messages.Select(x => x.Author.Id).Distinct().ToList();
            var channels = messages.Select(x => x.Channel.Id).Distinct().ToList();
            var tasks = this.GetTimeRangeMovePerPeriod(period, oldestMessageDatetime)
                .Select(timeRange => this.ProcessTimeRangeMessages(serverId, messages, timeRange, period, users, channels))
                .ToArray();
            Task.WaitAll(tasks);
            return this.SaveChanges();
        }

        private Task ProcessTimeRangeMessages(ulong serverId, IEnumerable<Message> messages, TimeRange timeRange, string period, List<ulong> users, List<ulong> channels) //todo test
        {
            var messagesInTimeRange = messages.Where(x => timeRange.Contains(x.SentAt)).ToList();
            if (messagesInTimeRange.Count == 0)
            {
                return Task.CompletedTask;
            }
            this.SaveStatisticCommand(serverId: serverId, userId: 0, channelId: 0, messagesInTimeRange.Count, timeRange, period);
            foreach (var channel in channels)
            {
                this.ProcessChannels(serverId, channel, messagesInTimeRange, timeRange, users, period);
            }
            this.ProcessUsers(serverId, users, messagesInTimeRange, timeRange, period);
            return Task.CompletedTask;
        }

        private void ProcessChannels(ulong serverId, ulong channelId, IEnumerable<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, string period) //todo test
        {
            var messagesPerChannelInTimeRange = messagesInTimeRange.Where(x => x.Channel.Id == channelId).ToList();
            if (messagesPerChannelInTimeRange.Count == 0)
            {
                return;
            }
            this.SaveStatisticCommand(serverId: serverId, userId: 0, channelId: channelId, messagesPerChannelInTimeRange.Count, timeRange, period);
            foreach (var user in users)
            {
                var messagesPerUserAndChannelInTimeRange = messagesPerChannelInTimeRange.Where(x => x.Author.Id == user).ToList();
                if (messagesPerUserAndChannelInTimeRange.Count == 0)
                {
                    continue;
                }
                this.SaveStatisticCommand(serverId: serverId, userId: user, channelId: channelId, messagesPerUserAndChannelInTimeRange.Count, timeRange, period);
            }
        }

        private void ProcessUsers(ulong serverId, List<ulong> users, IEnumerable<Message> messagesInTimeRange, TimeRange timeRange, string period) //todo test
        {
            foreach (var user in users)
            {
                var messagesPerUserInTimeRange = messagesInTimeRange.Where(x => x.Author.Id == user).ToList();
                if (messagesPerUserInTimeRange.Count == 0)
                {
                    continue;
                }
                this.SaveStatisticCommand(serverId: serverId, userId: user, channelId: 0, messagesPerUserInTimeRange.Count(), timeRange, period);
            }
        }

        private IEnumerable<Message> GetMessages(ulong serverId)
        {
            var query = new GetMessagesQuery(serverId);
            var messages = this.queryBus.Execute(query).Messages.ToList();
            return messages;
        }

        private void SaveStatisticCommand(ulong serverId, ulong userId, ulong channelId, int count, TimeRange timeRange, string period) //todo test
        {
            var preGeneratedStatistic = new PreGeneratedStatistic(serverId, count, timeRange, period);
            preGeneratedStatistic.SetUser(userId);
            preGeneratedStatistic.SetChannel(channelId);
            this.preGeneratedStatistics.Add(preGeneratedStatistic);
        }

        private async Task SaveChanges()
        {
            var command = new AddOrUpdatePreGeneratedStatisticsCommand(this.preGeneratedStatistics);
            await this.commandBus.ExecuteAsync(command);
            this.preGeneratedStatistics = new List<PreGeneratedStatistic>();
        }

        private IEnumerable<TimeRange> GetTimeRangeMovePerPeriod(string period, DateTime oldestMessageDatetime) //TODO test
        {
            var startOfCurrentPeriod = period switch
            {
                Period.Day => DateTime.Today,
                Period.Month => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                Period.Quarter => this.GetQuarterStart(DateTime.Today),
                _ => throw new NotImplementedException()
            };
            var moveForward = period switch
            {
                Period.Day => new Func<DateTime, int>(x => 1),
                Period.Month => new Func<DateTime, int>(x => DateTime.DaysInMonth(x.Year, x.Month)),
                Period.Quarter => new Func<DateTime, int>(x => new DateTime[] { x, x.AddMonths(1), x.AddMonths(2) }.Select(d => DateTime.DaysInMonth(d.Year, d.Month)).Sum()),
                _ => throw new NotImplementedException()
            };
            var moveBackward = period switch
            {
                Period.Day => new Func<DateTime, int>(x => 1),
                Period.Month => new Func<DateTime, int>(x => DateTime.DaysInMonth(x.AddMonths(-1).Year, x.AddMonths(-1).Month)),
                Period.Quarter => new Func<DateTime, int>(x => new DateTime[] { x.AddMonths(-1), x.AddMonths(-2), x.AddMonths(-3) }.Select(d => DateTime.DaysInMonth(d.Year, d.Month)).Sum()),
                _ => throw new NotImplementedException()
            };
            var minusDaysAtEnd = period switch
            {
                Period.Day => 0,
                Period.Month => 1,
                Period.Quarter => 1,
                _ => throw new NotImplementedException()
            };

            var periodTimeRange = TimeRange.Create(startOfCurrentPeriod, startOfCurrentPeriod.AddDays(moveForward.Invoke(startOfCurrentPeriod) - minusDaysAtEnd).AddMilliseconds(-1));
            periodTimeRange = periodTimeRange.Move(TimeSpan.FromDays(-moveBackward.Invoke(startOfCurrentPeriod)));
            var iterableTimeRange = periodTimeRange.MoveWhile(x => !x.Contains(oldestMessageDatetime), x => TimeSpan.FromDays(-moveBackward.Invoke(x.Start)));
            return iterableTimeRange;
        }

        private DateTime GetQuarterStart(DateTime date) //TODO test
        {
            return date.Month switch
            {
                int month when month <= 3 => new DateTime(date.Year, 1, 1),
                int month when month <= 6 => new DateTime(date.Year, 4, 1),
                int month when month <= 9 => new DateTime(date.Year, 7, 1),
                _ => new DateTime(date.Year, 10, 1)
            };
        }
    }
}
