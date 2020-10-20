using Statsman.Core.Generators.Models;
using Statsman.Core.Generators.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Commands;
using Watchman.DomainModel.Messages.Queries;

namespace Statsman.Core.Generators
{
    public class PreGeneratedStatisticsGenerator
    {
        private readonly IQueryBus queryBus;
        private readonly StatisticsTimeService _statisticsTimeService;
        private readonly StatisticsStorageService _statisticsStorageService;
        private readonly StatisticsProcessingService _statisticsProcessingService;

        public PreGeneratedStatisticsGenerator(IQueryBus queryBus, StatisticsTimeService statisticsTimeService, StatisticsStorageService statisticsStorageService, StatisticsProcessingService statisticsProcessingService)
        {
            this.queryBus = queryBus;
            this._statisticsTimeService = statisticsTimeService;
            this._statisticsStorageService = statisticsStorageService;
            this._statisticsProcessingService = statisticsProcessingService;
        }

        public Task ProcessStatisticsPerPeriod(ulong serverId, string period) 
        {
            var messages = this.GetMessages(serverId);
            var oldestMessageDatetime = messages.Any() ? messages.Min(x => x.SentAt) : default;
            if (oldestMessageDatetime == default) //empty database
            {
                return Task.CompletedTask;
            }
            var users = messages.Select(x => x.Author.Id).Distinct().ToList();
            var channels = messages.Select(x => x.Channel.Id).Distinct().ToList();
            var tasks = _statisticsTimeService.GetTimeRangeMovePerPeriod(period, oldestMessageDatetime).Select(timeRange => this.ProcessTimeRangeMessages(serverId, messages, timeRange, period, users, channels));
            Task.WaitAll(tasks.ToArray());
            return this._statisticsStorageService.SaveChanges();
        }

        private Task ProcessTimeRangeMessages(ulong serverId, IReadOnlyList<Message> messages, TimeRange timeRange, string period, List<ulong> users, List<ulong> channels)
        {
            return Task.Run(() => 
            {
                var messagesToSave = _statisticsProcessingService.ProcessEverythingInTimeRange(serverId, messages, timeRange, users, channels, period);
                foreach (var item in messagesToSave)
                {
                    this._statisticsStorageService.SaveStatisticCommand(item);
                }
            });
        }

        private IReadOnlyList<Message> GetMessages(ulong serverId)
        {
            var query = new GetMessagesQuery(serverId);
            var messages = this.queryBus.Execute(query).Messages.ToList();
            return messages;
        }
    }
}
