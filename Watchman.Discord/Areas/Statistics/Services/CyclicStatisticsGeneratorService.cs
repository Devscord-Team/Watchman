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
    public class CyclicStatisticsGeneratorService
    {
        private static bool _shouldStillGenerateEveryday;
        private static bool _isNowRunningCyclicGenerator;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly DiscordServersService _discordServersService;

        public CyclicStatisticsGeneratorService(IQueryBus queryBus, ICommandBus commandBus, DiscordServersService discordServersService)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
            _discordServersService = discordServersService;
        }

        public async Task StartGeneratingStatsCacheEveryday()
        {
            if (_isNowRunningCyclicGenerator)
            {
                return;
            }
            _isNowRunningCyclicGenerator = true;
            _shouldStillGenerateEveryday = true;
            while (_shouldStillGenerateEveryday)
            {
                await BlockUntilNextNight();
                await GenerateStatsForLastDay();
            }
            _isNowRunningCyclicGenerator = false;
        }

        public Task StopGeneratingStatsCacheEveryday() // after calling this method, stopping will happen after next day generating - after stopping it will generate One more stats cache
        {
            _shouldStillGenerateEveryday = false;
            return Task.CompletedTask;
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

        private async Task BlockUntilNextNight()
        {
            const int hourWhenShouldGenerateCyclicStatistics = 02; // 24h clock

            var nightTimeThisDay = DateTime.Today.AddHours(hourWhenShouldGenerateCyclicStatistics); // always 2:00AM this day
            var nextNight = DateTime.Now.Hour < hourWhenShouldGenerateCyclicStatistics
                ? nightTimeThisDay
                : nightTimeThisDay.AddDays(1);

            var delay = nextNight - DateTime.Now;
            await Task.Delay(delay);
        }

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
