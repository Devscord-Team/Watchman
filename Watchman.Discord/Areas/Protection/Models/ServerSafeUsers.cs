using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.DomainModel.Messages;

namespace Watchman.Discord.Areas.Protection.Models
{
    public readonly struct ServerSafeUsers
    {
        public ulong ServerId { get; }
        public HashSet<ulong> SafeUsers { get; }

        public ServerSafeUsers(IEnumerable<Message> serverMessages, ulong serverId, int minAverageMessagesPerWeek, HashSet<ulong> trustedRolesIds, UsersService usersService, DiscordServersService discordServersService)
        {
            this.ServerId = serverId;
            var users = discordServersService.GetDiscordServerAsync(serverId).Result.GetUsers().ToDictionaryAsync(x => x.Id, x => x).Result;
            this.SafeUsers = serverMessages
                .GroupBy(x => x.Author.Id)
                .Where(u => IsUserSafe(u.ToList(), users.GetValueOrDefault(u.Key), serverId, minAverageMessagesPerWeek, trustedRolesIds, usersService))
                .Select(x => x.Key)
                .ToHashSet();
        }

        private static bool IsUserSafe(IReadOnlyCollection<Message> userMessages, UserContext user, ulong serverId, int minAverageMessagesPerWeek, HashSet<ulong> trustedRolesIds, UsersService usersService)
        {
            if (user == null || userMessages.Count < minAverageMessagesPerWeek)
            {
                return false;
            }
            var days = GetHowManyDaysUserIsOnThisServer(user.Id, serverId, usersService);
            if (days < 30)
            {
                return false;
            }
            if (user.Roles.Any(x => trustedRolesIds.Contains(x.Id)))
            {
                return true;
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

            var avg = cutWeeks.Sum() / (days / 7);
            return avg > minAverageMessagesPerWeek;
        }

        private static int GetHowManyDaysUserIsOnThisServer(ulong userId, ulong serverId, UsersService usersService)
        {
            var joinedAt = usersService.GetUserJoinedServerAt(userId, serverId) ?? DateTime.Now;
            return (int)(DateTime.Now.Date - joinedAt.Date).TotalDays;
        }

        private static DateTime StartOfWeek(DateTime date)
        {
            return date.AddDays(-1 * (int)date.DayOfWeek).Date;
        }
    }
}