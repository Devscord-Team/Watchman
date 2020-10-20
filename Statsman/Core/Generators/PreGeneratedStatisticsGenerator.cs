using Statsman.Core.Generators.Services;

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
        private List<PreGeneratedStatistic> _preGeneratedStatistics = new List<PreGeneratedStatistic>();

        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;
        private readonly StatisticsTimeService _statisticsTimeService;

        public PreGeneratedStatisticsGenerator(IQueryBus queryBus, ICommandBus commandBus, StatisticsTimeService statisticsTimeService)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
            this._statisticsTimeService = statisticsTimeService;
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
            var oldestMessageDatetime = messages.Any() ? messages.Min(x => x.SentAt) : default;
            if (oldestMessageDatetime == default) //empty database
            {
                return Task.CompletedTask;
            }
            var users = messages.Select(x => x.Author.Id).Distinct().ToList();
            var channels = messages.Select(x => x.Channel.Id).Distinct().ToList();
            var tasks = _statisticsTimeService.GetTimeRangeMovePerPeriod(period, oldestMessageDatetime)
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
            this._preGeneratedStatistics.Add(preGeneratedStatistic);
        }

        private async Task SaveChanges()
        {
            var command = new AddOrUpdatePreGeneratedStatisticsCommand(this._preGeneratedStatistics);
            await this.commandBus.ExecuteAsync(command);
            this._preGeneratedStatistics = new List<PreGeneratedStatistic>();
        }
    }
}
