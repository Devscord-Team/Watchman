using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages.Queries;

namespace Statsman
{
    public class StatisticsGenerator
    {
        private readonly IQueryBus queryBus;

        public StatisticsGenerator(IQueryBus queryBus)
        {
            this.queryBus = queryBus;
        }

        public async Task PerHour(ulong serverId, TimeSpan timeBehind)
        {
        }

        public async Task PerDay(ulong serverId, TimeSpan timeBehind)
        {
            var query = this.GetQuery(serverId, timeBehind);
            var preCalculatedDays = await this.queryBus.ExecuteAsync(query);
        }

        public async Task PerWeek(ulong serverId, TimeSpan timeBehind)
        {
            var query = this.GetQuery(serverId, timeBehind);
            var preCalculatedDays = await this.queryBus.ExecuteAsync(query);
        }

        public async Task PerMonth(ulong serverId, TimeSpan timeBehind)
        {
            var query = this.GetQuery(serverId, timeBehind);
            var preCalculatedDays = await this.queryBus.ExecuteAsync(query);
        }

        public async Task PerQuarter(ulong serverId, TimeSpan timeBehind)
        {
            var query = this.GetQuery(serverId, timeBehind);
            var preCalculatedDays = await this.queryBus.ExecuteAsync(query);
        }

        private GetServerDayStatisticsQuery GetQuery(ulong serverId, TimeSpan timeBehind)
        {
            var query = new GetServerDayStatisticsQuery(serverId)
            {
                SentDate = TimeRange.Create(DateTime.Today.AddDays(timeBehind.TotalDays), DateTime.Today)
            };
            return query;
        }
    }
}
