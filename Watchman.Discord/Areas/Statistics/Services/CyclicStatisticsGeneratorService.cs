using System;
using System.Linq;
using System.Threading.Tasks;

using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;

using Serilog;

using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Commands;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.Areas.Statistics.Services
{
    public class CyclicStatisticsGeneratorService : ICyclicService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly DiscordServersService _discordServersService;

        public CyclicStatisticsGeneratorService(IQueryBus queryBus, ICommandBus commandBus, DiscordServersService discordServersService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._discordServersService = discordServersService;
        }

        public async Task GenerateStatsForDaysBefore(DiscordServerContext server, DateTime? lastInitDate)
        {
            var dayStatisticsQuery = new GetServerDayStatisticsQuery(server.Id);
            var allServerDaysStatistics = (await this._queryBus.ExecuteAsync(dayStatisticsQuery)).ServerDayStatistics.ToList();
            var startDate = lastInitDate ?? DateTime.UnixEpoch;
            var messagesQuery = new GetMessagesQuery(server.Id)
            {
                SentDate = new TimeRange(startDate, DateTime.Today) // it will exclude today - it should generate today's stats tomorrow
            };
            var messages = this._queryBus.Execute(messagesQuery).Messages;

            var messagesNotCachedForStats = messages
                .Where(message => allServerDaysStatistics.All(s => message.SentAt.Date != s.Date));

            var serverStatistics = messagesNotCachedForStats
                .GroupBy(x => x.SentAt.Date)
                .Select(x => new ServerDayStatistic(x.ToList(), server.Id, x.Key));

            var commands = serverStatistics.Select(x => new AddServerDayStatisticCommand(x));
            foreach (var command in commands)
            {
                await this._commandBus.ExecuteAsync(command);
            }
        }

        public async Task Refresh()
        {
            Log.Information("Refreshing stats...");
            await this.GenerateStatsForLastDay();
            Log.Information("Stats refreshed");
        }

        private async Task GenerateStatsForLastDay()
        {
            var servers = this._discordServersService.GetDiscordServersAsync();
            var yesterdayDate = DateTime.Today.AddDays(-1);
            var todayDate = DateTime.Today;
            var yesterdayRange = new TimeRange(yesterdayDate, todayDate);

            await foreach (var server in servers)
            {
                var query = new GetMessagesQuery(server.Id) { SentDate = yesterdayRange };
                var messages = this._queryBus.Execute(query).Messages.ToList();

                var serverStatistic = new ServerDayStatistic(messages, server.Id, DateTime.Today);
                var command = new AddServerDayStatisticCommand(serverStatistic);
                await this._commandBus.ExecuteAsync(command);
            }
        }
    }
}
