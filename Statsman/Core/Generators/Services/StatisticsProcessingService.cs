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


        public IEnumerable<SaveStatisticItem> ProcessTimeRangeMessages(ulong serverId, IReadOnlyList<Message> messages, TimeRange timeRange, string period, List<ulong> users, List<ulong> channels)
        {
            var messagesInTimeRange = messages.Where(x => timeRange.Contains(x.SentAt)).ToList();
            if (messagesInTimeRange.Count == 0)
            {
                yield return null;
            }
            yield return new SaveStatisticItem(serverId: serverId, userId: 0, channelId: 0, messagesInTimeRange.Count, timeRange, period);
            
            var channelsItemsToSave = this.ProcessChannels(serverId, messagesInTimeRange, timeRange, users, channels, period);
            foreach (var item in channelsItemsToSave)
            {
                yield return item;
            }
            
            var usersItemsToSave = this.ProcessUsers(serverId, users, messagesInTimeRange, timeRange, period);
            foreach (var item in usersItemsToSave)
            {
                yield return item;
            }
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

                var usersForChannelItemsToSave = this.ProcessUsersOnChannel(serverId, channel, messagesInTimeRange, timeRange, users, period);
                foreach (var item in usersForChannelItemsToSave)
                {
                    yield return item;
                }
            }
        }

        public IEnumerable<SaveStatisticItem> ProcessUsersOnChannel(ulong serverId, ulong channelId, IReadOnlyList<Message> messagesInTimeRange, TimeRange timeRange, List<ulong> users, string period)
        {
            foreach (var user in users)
            {
                var messagesPerUserAndChannelInTimeRange = messagesInTimeRange.Where(x => x.Author.Id == user).ToList();
                if (messagesPerUserAndChannelInTimeRange.Count == 0)
                {
                    continue;
                }
                yield return new SaveStatisticItem(serverId: serverId, userId: user, channelId: channelId, messagesPerUserAndChannelInTimeRange.Count, timeRange, period);
            }
        }

        public IEnumerable<SaveStatisticItem> ProcessUsers(ulong serverId, List<ulong> users, IReadOnlyList<Message> messagesInTimeRange, TimeRange timeRange, string period)
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
