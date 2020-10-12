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
            var messages = this.GetMessages(serverId);
            var preGeneratedStatistics = this.GetPreGeneratedStatistics(serverId, Period.Day);
            var oldestMessageDatetime = preGeneratedStatistics.OrderBy(x => x.TimeRange.End).FirstOrDefault()?.TimeRange?.End 
                ?? messages.OrderBy(x => x.SentAt).FirstOrDefault()?.SentAt 
                ?? default;
            if(oldestMessageDatetime == default) //empty database
            {
                return;
            }

            var users = messages.Select(x => x.Author.Id).Distinct().ToList();
            var channels = messages.Select(x => x.Channel.Id).Distinct().ToList();

            var todayTimeRange = TimeRange.Create(DateTime.Today, DateTime.Today.AddSeconds(-1)).Move(TimeSpan.FromDays(-1)); //don't calculate today
            var iterableTimeRange = todayTimeRange.MoveWhile(x => !x.Contains(oldestMessageDatetime), TimeSpan.FromDays(1));
            foreach (var timeRange in iterableTimeRange)
            {
                var messagesInTimeRange = messages.Where(x => timeRange.Contains(x.SentAt)).ToList();
                await this.SaveStatistic(serverId: serverId, userId: 0, channelId: 0, messagesInTimeRange.Count, timeRange, Period.Day);
                foreach (var channel in channels)
                {
                    var messagesPerChannelInTimeRange = messagesInTimeRange.Where(x => x.Channel.Id == channel).ToList();
                    await this.SaveStatistic(serverId: serverId, userId: 0, channelId: channel, messagesPerChannelInTimeRange.Count, timeRange, Period.Day);
                    foreach (var user in users)
                    {
                        var messagesPerUserAndChannelInTimeRange = messagesPerChannelInTimeRange.Where(x => x.Author.Id == user);
                        await this.SaveStatistic(serverId: serverId, userId: user, channelId: channel, messagesPerUserAndChannelInTimeRange.Count(), timeRange, Period.Day);
                    }
                }
                foreach (var user in users)
                {
                    var messagesPerUserInTimeRange = messagesInTimeRange.Where(x => x.Author.Id == user);
                    await this.SaveStatistic(serverId: serverId, userId: user, channelId: 0, messagesPerUserInTimeRange.Count(), timeRange, Period.Day);
                }
            }
        }

        public async Task PreGenerateStatisticsPerMonth(ulong serverId)
        {
            var messages = this.GetMessages(serverId);
        }

        public async Task PreGenerateStatisticsPerQuarter(ulong serverId)
        {
            var messages = this.GetMessages(serverId);
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
    }
}
