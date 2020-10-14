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
    public class PreReneratedStatisticsGenerator
    {
        public IQueryBus QueryBus { get; private set; }
        public ICommandBus CommandBus { get; private set; }

        public PreReneratedStatisticsGenerator(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.QueryBus = queryBus;
            this.CommandBus = commandBus;
        }

        public async Task PreGenerateStatisticsPerDay(ulong serverId) //todo test
        {
            await this.ProcessStatisticsPerPeriod(serverId, Period.Day);
        }

        public async Task PreGenerateStatisticsPerMonth(ulong serverId) //todo test
        {
            await this.ProcessStatisticsPerPeriod(serverId, Period.Month);
        }

        public async Task PreGenerateStatisticsPerQuarter(ulong serverId) //todo test
        {
            await this.ProcessStatisticsPerPeriod(serverId, Period.Quarter);
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
            return Task.CompletedTask;
        }

        private async Task ProcessTimeRangeMessages(ulong serverId, IEnumerable<Message> messages, TimeRange timeRange, string period, List<ulong> users, List<ulong> channels) //todo test
        {
            var messagesInTimeRange = messages.Where(x => timeRange.Contains(x.SentAt)).ToList();
            if (messagesInTimeRange.Count == 0)
            {
                return;
            }
            await this.SaveStatistic(serverId: serverId, userId: 0, channelId: 0, messagesInTimeRange.Count, timeRange, period);
            foreach (var channel in channels)
            {
                await this.ProcessChannels(serverId, channel, messagesInTimeRange, timeRange, users, period);
            }
            await this.ProcessUsers(serverId, users, messagesInTimeRange, timeRange, period);
        }

        private async Task ProcessChannels(ulong serverId, ulong channelId, IEnumerable<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, string period) //todo test
        {
            var messagesPerChannelInTimeRange = messagesInTimeRange.Where(x => x.Channel.Id == channelId).ToList();
            if (messagesPerChannelInTimeRange.Count == 0)
            {
                return;
            }
            await this.SaveStatistic(serverId: serverId, userId: 0, channelId: channelId, messagesPerChannelInTimeRange.Count, timeRange, period);
            foreach (var user in users)
            {
                var messagesPerUserAndChannelInTimeRange = messagesPerChannelInTimeRange.Where(x => x.Author.Id == user).ToList();
                if (messagesPerUserAndChannelInTimeRange.Count == 0)
                {
                    continue;
                }
                await this.SaveStatistic(serverId: serverId, userId: user, channelId: channelId, messagesPerUserAndChannelInTimeRange.Count, timeRange, period);
            }
        }

        private async Task ProcessUsers(ulong serverId, List<ulong> users, IEnumerable<Message> messagesInTimeRange, TimeRange timeRange, string period) //todo test
        {
            foreach (var user in users)
            {
                var messagesPerUserInTimeRange = messagesInTimeRange.Where(x => x.Author.Id == user).ToList();
                if (messagesPerUserInTimeRange.Count == 0)
                {
                    continue;
                }
                await this.SaveStatistic(serverId: serverId, userId: user, channelId: 0, messagesPerUserInTimeRange.Count(), timeRange, period);
            }
        }

        private IEnumerable<Message> GetMessages(ulong serverId)
        {
            var query = new GetMessagesQuery(serverId);
            var messages = this.QueryBus.Execute(query).Messages.ToList();
            return messages;
        }

        private async Task SaveStatistic(ulong serverId, ulong userId, ulong channelId, int count, TimeRange timeRange, string period) //todo test
        {
            var preGeneratedStatistic = new PreGeneratedStatistic(serverId, count, timeRange, period);
            preGeneratedStatistic.SetUser(userId);
            preGeneratedStatistic.SetChannel(channelId);
            var preGeneratedStatisticCommand = new AddOrUpdatePreGeneratedStatisticCommand(preGeneratedStatistic);
            await this.CommandBus.ExecuteAsync(preGeneratedStatisticCommand);
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
                Period.Day => moveForward,
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
            
            var periodTimeRange = TimeRange.Create(startOfCurrentPeriod, startOfCurrentPeriod.AddDays(moveForward.Invoke(startOfCurrentPeriod) - minusDaysAtEnd).AddMilliseconds(-1))
                .Move(TimeSpan.FromDays(-moveBackward.Invoke(startOfCurrentPeriod)));
            var iterableTimeRange = periodTimeRange.MoveWhile(x => !x.Contains(oldestMessageDatetime), x => TimeSpan.FromDays(-moveBackward.Invoke(x.Start)));
            return iterableTimeRange;
        }

        private DateTime GetQuarterStart(DateTime date) //TODO test
        {
            if (date.Month <= 3)
            {
                return new DateTime(date.Year, 1, 1);
            }
            else if (date.Month <= 6)
            {
                return new DateTime(date.Year, 4, 1);
            }
            else if (date.Month <= 9)
            {
                return new DateTime(date.Year, 7, 1);
            }
            else
            {
                return new DateTime(date.Year, 10, 1);
            }
        }
    }
}
