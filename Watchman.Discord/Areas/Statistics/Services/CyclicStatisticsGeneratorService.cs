using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.Discord.Areas.Statistics.Services
{
    public class CyclicStatisticsGeneratorService
    {
        private static bool _shouldStillGenerateEveryday;
        private readonly IQueryBus _queryBys;
        private readonly ICommandBus _commandBus;

        public CyclicStatisticsGeneratorService(IQueryBus queryBys, ICommandBus commandBus)
        {
            _queryBys = queryBys;
            _commandBus = commandBus;
        }

        public async Task StartGeneratingStatsCacheEveryday()
        {
            while (_shouldStillGenerateEveryday)
            {
                await BlockUntilNextNight();
                await GenerateStatsForLastDay();
            }
        }

        public Task StopGeneratingStatsCacheEveryday()
        {
            _shouldStillGenerateEveryday = false;
            return Task.CompletedTask;
        }

        private async Task BlockUntilNextNight()
        {
            var nightTimeThisDay = DateTime.Now.Date.AddHours(2); // always 2:00AM this day
            var nextNight = DateTime.Now.Hour < 2
                ? nightTimeThisDay
                : nightTimeThisDay.AddDays(1);

            var delay = nextNight - DateTime.Now;
            await Task.Delay(delay);
        }

        private Task GenerateStatsForLastDay()
        {
            return Task.CompletedTask;
        }
    }
}
