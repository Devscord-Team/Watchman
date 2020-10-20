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
            if (messagesInTimeRange.Count == 0)
            {
                yield return null;
            }
            yield return new SaveStatisticItem(serverId: serverId, userId: 0, channelId: 0, messagesInTimeRange.Count, timeRange, period);
        }

        public IEnumerable<SaveStatisticItem> ProcessChannels(ulong serverId, IReadOnlyList<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, List<ulong> channels, string period)
        {
            foreach (var channel in channels)
            {
                var messagesPerChannelInTimeRange = messagesInTimeRange.Where(x => x.Channel.Id == channel).ToList();
                if (messagesPerChannelInTimeRange.Count == 0)
                {
                    yield return null;
                }
                yield return new SaveStatisticItem(serverId: serverId, userId: 0, channelId: channel, messagesPerChannelInTimeRange.Count, timeRange, period);

                var usersForChannelItemsToSave = this.ProcessUsersOnChannel(serverId, channel, messagesPerChannelInTimeRange, timeRange, users, period);
                foreach (var item in usersForChannelItemsToSave)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<SaveStatisticItem> ProcessUsersOnChannel(ulong serverId, ulong channelId, IReadOnlyList<Message> messagesInChannelInTimeRange, TimeRange timeRange, List<ulong> users, string period)
        {
            foreach (var user in users)
            {
                var messagesPerUserAndChannelInTimeRange = messagesInChannelInTimeRange.Where(x => x.Author.Id == user).ToList();
                if (messagesPerUserAndChannelInTimeRange.Count == 0)
                {
                    continue;
                }
                yield return new SaveStatisticItem(serverId: serverId, userId: user, channelId: channelId, messagesPerUserAndChannelInTimeRange.Count, timeRange, period);
            }
        }

        public IEnumerable<SaveStatisticItem> ProcessUsers(ulong serverId, IReadOnlyList<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, string period)
        {
            foreach (var user in users)
            {
                var messagesPerUserInTimeRange = messagesInTimeRange.Where(x => x.Author.Id == user).ToList();
                if (messagesPerUserInTimeRange.Count == 0)
                {
                    continue;
                }
                yield return new SaveStatisticItem(serverId: serverId, userId: user, channelId: 0, messagesPerUserInTimeRange.Count(), timeRange, period);
            }
        }
    }
}
