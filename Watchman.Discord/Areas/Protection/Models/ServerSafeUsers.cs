using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Services;
using Watchman.DomainModel.Messages;

namespace Watchman.Discord.Areas.Protection.Models
{
    public struct ServerSafeUsers
    {
        public static UsersService UsersService { get; set; }

        public ulong ServerId { get; }
        public HashSet<ulong> SafeUsers { get; private set; }

        public ServerSafeUsers(IEnumerable<Message> serverMessages, ulong serverId, int minAverageMessagesPerWeek)
        {
            ServerId = serverId;

            this.SafeUsers = serverMessages
                .GroupBy(x => x.Author.Id)
                .Where(u => IsUserSafe(u, u.Key, serverId, minAverageMessagesPerWeek))
                .Select(x => x.Key)
                .ToHashSet();
        }

        private static bool IsUserSafe(IEnumerable<Message> userMessages, ulong userId, ulong serverId, int minAverageMessagesPerWeek)
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
            return avg > minAverageMessagesPerWeek;
        }

        private static int GetHowManyDaysUserIsOnThisServer(ulong userId, ulong serverId)
        {
            var joinedAt = UsersService.GetUserJoinedDateTime(userId, serverId) ?? DateTime.Now;
            return (int)(DateTime.Now.Date - joinedAt.Date).TotalDays;
        }

        private static DateTime StartOfWeek(DateTime date)
        {
            return date.AddDays(-1 * (int)date.DayOfWeek).Date;
        }
    }
}