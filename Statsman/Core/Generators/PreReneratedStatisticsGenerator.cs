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

        public async Task PreGenerateStatisticsPerDay(ulong serverId)
        {
            await this.ProcessStatisticsPerPeriod(serverId, Period.Day);
        }

        public async Task PreGenerateStatisticsPerMonth(ulong serverId)
        {
            await this.ProcessStatisticsPerPeriod(serverId, Period.Month);
        }

        public async Task PreGenerateStatisticsPerQuarter(ulong serverId)
        {
            await this.ProcessStatisticsPerPeriod(serverId, Period.Quarter);
        }

        public async Task ProcessStatisticsPerPeriod(ulong serverId, string period)
        {
            var messages = this.GetMessages(serverId);
            var preGeneratedStatistics = this.GetPreGeneratedStatistics(serverId, Period.Day);
            var oldestMessageDatetime = preGeneratedStatistics.OrderBy(x => x.TimeRange.End).FirstOrDefault()?.TimeRange?.End
                ?? messages.OrderBy(x => x.SentAt).FirstOrDefault()?.SentAt
                ?? default;
            if (oldestMessageDatetime == default) //empty database
            {
                return;
            }
            var users = messages.Select(x => x.Author.Id).Distinct().ToList();
            var channels = messages.Select(x => x.Channel.Id).Distinct().ToList();

            var iterableTimeRange = this.GetTimeRangeMovePerPeriod(Period.Day, oldestMessageDatetime);
            foreach (var timeRange in iterableTimeRange)
            {
                await this.ProcessTimeRangeMessages(serverId, messages, timeRange, Period.Day, users, channels);
            }
        }

        private async Task ProcessTimeRangeMessages(ulong serverId, IEnumerable<Message> messages, TimeRange timeRange, string period, List<ulong> users, List<ulong> channels)
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
            await ProcessUsers(serverId, users, messagesInTimeRange, timeRange, period);
        }

        private async Task ProcessChannels(ulong serverId, ulong channelId, IEnumerable<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, string period)
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
                if(messagesPerUserAndChannelInTimeRange.Count == 0)
                {
                    continue;
                }
                await this.SaveStatistic(serverId: serverId, userId: user, channelId: channelId, messagesPerUserAndChannelInTimeRange.Count, timeRange, period);
            }
        }

        private async Task ProcessUsers(ulong serverId, List<ulong> users, IEnumerable<Message> messagesInTimeRange, TimeRange timeRange, string period)
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

        private IEnumerable<PreGeneratedStatistic> GetPreGeneratedStatistics(ulong serverId, string period)
        {
            var query = new GetPreGeneratedStatisticQuery(serverId, period: period);
            var preGeneratedStatistics = this.QueryBus.Execute(query).PreGeneratedStatistic.ToList();
            return preGeneratedStatistics;
        }

        private async Task SaveStatistic(ulong serverId, ulong userId, ulong channelId, int count, TimeRange timeRange, string period)
        {
            var preGeneratedStatistic = new PreGeneratedStatistic(serverId, count, timeRange, Period.Day);
            preGeneratedStatistic.SetUser(userId);
            preGeneratedStatistic.SetChannel(channelId);
            var preGeneratedStatisticCommand = new AddPreGeneratedStatisticCommand(preGeneratedStatistic);
            await this.CommandBus.ExecuteAsync(preGeneratedStatisticCommand);
        }

        private IEnumerable<TimeRange> GetTimeRangeMovePerPeriod(string period, DateTime oldestMessageDatetime)
        {
            var days = period switch
            {
                Period.Day => 1,
                Period.Month => 30,
                Period.Quarter => 90,
                _ => throw new NotImplementedException()
            };
            var todayTimeRange = TimeRange.Create(DateTime.Today, DateTime.Today.AddDays(days).AddSeconds(-1)).Move(TimeSpan.FromDays(-days)); //don't calculate today
            var iterableTimeRange = todayTimeRange.MoveWhile(x => !x.Contains(oldestMessageDatetime), TimeSpan.FromDays(days));
            return iterableTimeRange;
        }
    }
}
