using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
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
        private readonly IQueryBus _queryBys;
        private readonly ICommandBus _commandBus;
        private readonly DiscordServersService _discordServersService;

        public CyclicStatisticsGeneratorService(IQueryBus queryBys, ICommandBus commandBus, DiscordServersService discordServersService)
        {
            _queryBys = queryBys;
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
            while (_shouldStillGenerateEveryday)
            {
                await BlockUntilNextNight();
                await GenerateStatsForLastDay();
            }
            _isNowRunningCyclicGenerator = false;
        }

        public Task StopGeneratingStatsCacheEveryday()
        {
            _shouldStillGenerateEveryday = false;
            return Task.CompletedTask;
        }

        public async Task GenerateStatsForDaysBefore(DiscordServerContext server)
        {
            // get all server cached stats
            var query = new GetServerDayStatisticsQuery(server.Id);
            // select (by date) only new ones

            // write to base the new ones
        }

        private async Task BlockUntilNextNight()
        {
            const int hourWhenShouldGenerateCyclicStatistics = 2; // 24h clock

            var nightTimeThisDay = DateTime.Now.Date.AddHours(hourWhenShouldGenerateCyclicStatistics); // always 2:00AM this day
            var nextNight = DateTime.Now.Hour < hourWhenShouldGenerateCyclicStatistics
                ? nightTimeThisDay
                : nightTimeThisDay.AddDays(1);

            var delay = nextNight - DateTime.Now;
            await Task.Delay(delay);
        }

        private async Task GenerateStatsForLastDay()
        {
            var servers = await _discordServersService.GetDiscordServers();
            foreach (var server in servers)
            {
                var query = new GetMessagesQuery(server.Id);
                //todo: filter one day
                var messages = _queryBys.Execute(query).Messages.ToList();
                var serverStatistic = new ServerDayStatistic(messages, server.Id);
                var command = new AddServerDayStatisticCommand(serverStatistic);
                await _commandBus.ExecuteAsync(command);
            }
        }
    }
}
