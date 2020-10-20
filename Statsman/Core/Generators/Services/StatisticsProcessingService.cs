using Statsman.Core.Generators.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.Common.Models;
using Watchman.DomainModel.Messages;

namespace Statsman.Core.Generators.Services
{
    public class StatisticsProcessingService
    {
        public IEnumerable<SaveStatisticItem> ProcessEverythingInTimeRange(ulong serverId, IReadOnlyList<Message> messages, TimeRange timeRange, List<ulong> users, List<ulong> channels, string period)
        {
            var messagesInTimeRange = messages.Where(x => timeRange.Contains(x.SentAt)).ToList();
            var results = new[]
            {
                this.ProcessGeneral(serverId, messagesInTimeRange, timeRange, period),
                this.ProcessChannels(serverId, messagesInTimeRange, timeRange, users, channels, period),
                this.ProcessUsers(serverId, messagesInTimeRange, timeRange, users, period),
            }.SelectMany(x => x);
            return results;
        }

        public IEnumerable<SaveStatisticItem> ProcessGeneral(ulong serverId, IReadOnlyList<Message> messagesInTimeRange, TimeRange timeRange, string period)
        {
            yield return this.GetStatisticItem(serverId: serverId, userId: 0, channelId: 0, timeRange, period, messagesInTimeRange, x => true);
        }

        public IEnumerable<SaveStatisticItem> ProcessChannels(ulong serverId, IReadOnlyList<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, List<ulong> channels, string period)
        {
            return channels.SelectMany(channel =>
            {
                var messagesPerChannelInTimeRange = messagesInTimeRange.Where(x => x.Channel.Id == channel).ToList();
                return new[]
                {
                    new List<SaveStatisticItem> { this.GetStatisticItem(serverId: serverId, userId: 0, channelId: channel, timeRange, period, messagesPerChannelInTimeRange, x => true) },
                    this.ProcessUsersOnChannel(serverId, channel, messagesPerChannelInTimeRange, timeRange, users, period)
                }.SelectMany(x => x);
            });
        }

        public IEnumerable<SaveStatisticItem> ProcessUsersOnChannel(ulong serverId, ulong channelId, IReadOnlyList<Message> messagesInChannelInTimeRange, TimeRange timeRange, List<ulong> users, string period)
        {
            return users.Select(user => this.GetStatisticItem(serverId: serverId, userId: user, channelId: channelId, timeRange, period, messagesInChannelInTimeRange, x => x.Author.Id == user));
        }

        public IEnumerable<SaveStatisticItem> ProcessUsers(ulong serverId, IReadOnlyList<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, string period)
        {
            return users.Select(user => this.GetStatisticItem(serverId: serverId, userId: user, channelId: 0, timeRange, period, messagesInTimeRange, x => x.Author.Id == user));
        }

        public SaveStatisticItem GetStatisticItem(ulong serverId, ulong userId, ulong channelId, TimeRange timeRange, string period, IReadOnlyList<Message> messages, Func<Message, bool> filter)
        {
            var filteredMessagesCount = messages.Where(filter).Count();
            return filteredMessagesCount == 0 ? null : new SaveStatisticItem(serverId, userId, channelId, filteredMessagesCount, timeRange, period);
        }
    }
}
