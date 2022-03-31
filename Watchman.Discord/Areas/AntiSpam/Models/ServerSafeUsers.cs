using System;
using System.Collections.Generic;
using System.Linq;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;

namespace Watchman.Discord.Areas.AntiSpam.Models
{
    /*
    public readonly struct ServerSafeUsers
    {
        public ulong ServerId { get; }
        public HashSet<ulong> SafeUsers { get; }

        public ServerSafeUsers(IEnumerable<Message> serverMessages, ulong serverId, int minAverageMessagesPerWeek, int minAbsoluteMessagesCount, HashSet<ulong> trustedRolesIds, IUsersService usersService, IDiscordServersService discordServersService)
        {
            this.ServerId = serverId;
            var server = discordServersService.GetDiscordServerAsync(serverId).GetAwaiter().GetResult();
            var users = server.GetUsers().ToDictionary(x => x.Id, x => x);
            this.SafeUsers = serverMessages
                .GroupBy(x => x.Author.Id)
                .Where(u => IsUserSafe(u.ToList(), users.GetValueOrDefault(u.Key), serverId, minAverageMessagesPerWeek, minAbsoluteMessagesCount, trustedRolesIds, usersService))
                .Select(x => x.Key)
                .ToHashSet();
        }

        private static bool IsUserSafe(IReadOnlyCollection<Message> userMessages, UserContext user, ulong serverId, int minAverageMessagesPerWeek, int minAbsoluteMessagesCount, HashSet<ulong> trustedRolesIds, IUsersService usersService)
        {
            if (user == null || userMessages.Count < minAbsoluteMessagesCount)
            {
                return false;
            }
            if (user.Roles.Any(x => trustedRolesIds.Contains(x.Id)))
            {
                return true;
            }
            var days = GetHowManyDaysUserIsOnThisServer(user.Id, serverId, usersService);
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

            var avg = cutWeeks.Sum() / (days / 7);
            return avg > minAverageMessagesPerWeek;
        }

        private static int GetHowManyDaysUserIsOnThisServer(ulong userId, ulong serverId, IUsersService usersService)
        {
            var joinedAt = usersService.GetUserJoinedServerAt(userId, serverId) ?? DateTime.Now;
            return (int)(DateTime.Now.Date - joinedAt.Date).TotalDays;
        }

        private static DateTime StartOfWeek(DateTime date)
        {
            return date.AddDays(-1 * (int)date.DayOfWeek).Date;
        }
    }
    */
}