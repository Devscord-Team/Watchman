using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Commands;
using Watchman.DomainModel.Messages.Queries;
using Watchman.DomainModel.Messages.Queries.Handlers;

namespace Watchman.Discord.Areas.Statistics.Services
{
    public class CyclicStatisticsGeneratorService : CyclicCacheGenerator
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly DiscordServersService _discordServersService;

        public CyclicStatisticsGeneratorService(IQueryBus queryBus, ICommandBus commandBus, DiscordServersService discordServersService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _discordServersService = discordServersService;
        }

        public async Task GenerateStatsForDaysBefore(DiscordServerContext server, DateTime? lastInitDate)
        {
            var dayStatisticsQuery = new GetServerDayStatisticsQuery(server.Id);
            var allServerDaysStatistics = (await _queryBus.ExecuteAsync(dayStatisticsQuery)).ServerDayStatistics.ToList();
            var startDate = lastInitDate ?? DateTime.UnixEpoch;
            var messagesQuery = new GetMessagesQuery(server.Id)
            {
                SentDate = new TimeRange(startDate, DateTime.Today) // it will exclude today - it should generate today's stats tomorrow
            };
            var messages = _queryBus.Execute(messagesQuery).Messages;

            var messagesNotCachedForStats = messages
                .Where(message => allServerDaysStatistics.All(s => message.SentAt.Date != s.Date));

            var serverStatistics = messagesNotCachedForStats
                .GroupBy(x => x.SentAt.Date)
                .Select(x => new ServerDayStatistic(x.ToList(), server.Id, x.Key));

            var commands = serverStatistics.Select(x => new AddServerDayStatisticCommand(x));
            foreach (var command in commands)
            {
                await _commandBus.ExecuteAsync(command);
            }
        }

        protected override async Task ReloadCache() => await GenerateStatsForLastDay();

        private async Task GenerateStatsForLastDay()
        {
            var servers = await _discordServersService.GetDiscordServers();
            var yesterdayDate = DateTime.Today.AddDays(-1);
            var todayDate = DateTime.Today;
            var yesterdayRange = new TimeRange(yesterdayDate, todayDate);

            foreach (var server in servers)
            {
                var query = new GetMessagesQuery(server.Id) { SentDate = yesterdayRange };
                var messages = _queryBus.Execute(query).Messages.ToList();

                var serverStatistic = new ServerDayStatistic(messages, server.Id, DateTime.Today);
                var command = new AddServerDayStatisticCommand(serverStatistic);
                await _commandBus.ExecuteAsync(command);
            }
        }
    }
}
