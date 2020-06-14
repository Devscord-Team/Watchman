using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Messages;

namespace Watchman.Discord.Areas.Protection.Models
{
    public class ServerSafeUsers
    {
        public ulong ServerId { get; }
        public List<ulong> SafeUsers { get; private set; }
        private readonly int _minAverageMessagesPerWeek;

        public ServerSafeUsers(IEnumerable<Message> serverMessages, ulong serverId, int minAverageMessagesPerWeek)
        {
            ServerId = serverId;
            _minAverageMessagesPerWeek = minAverageMessagesPerWeek;

            this.SafeUsers = serverMessages
                .GroupBy(x => x.Author.Id)
                .Where(u => IsUserSafe(u, u.Key, serverId))
                .Select(x => x.Key)
                .ToList();
        }

        private bool IsUserSafe(IEnumerable<Message> userMessages, ulong userId, ulong serverId)
        {
            var days = GetHowManyDaysUserIsOnThisServer(userId, serverId);
            if (days < 30)
            {
                return false;
            }

            var weeks = userMessages.GroupBy(x => StartOfWeek(x.SentAt))
                .Select(x => x.Count())
                .ToList();
            var sum = weeks.Sum();
            var cutWeeks = weeks.Select(x =>
            {
                var maxCountPerWeek = sum / 3;
                return x < maxCountPerWeek ? x : maxCountPerWeek;
            });

            var avg = cutWeeks.Sum() / days;
            return avg > this._minAverageMessagesPerWeek;
        }

        private int GetHowManyDaysUserIsOnThisServer(ulong userId, ulong serverId)
        {
            throw new NotImplementedException();
        }

        private DateTime StartOfWeek(DateTime date)
        {
            return date.AddDays(-1 * (int)date.DayOfWeek).Date;
        }
    }
}