using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Newtonsoft.Json;
using Serilog;

using Statsman;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Statistics.BotCommands;
using Watchman.Discord.Areas.Statistics.Models;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.DomainModel.Messages.Commands;
using Watchman.DomainModel.Messages.Queries;

namespace Watchman.Discord.Areas.Statistics.Controllers
{
    public class StatisticsController : IController
    {
        private readonly ChartsService _chartsService;
        private readonly StatisticsGenerator statisticsGenerator;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public StatisticsController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, ChartsService chartsService, StatisticsGenerator statisticsGenerator)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._chartsService = chartsService;
            this.statisticsGenerator = statisticsGenerator;
        }

        [ReadAlways]
        public async Task SaveMessageAsync(DiscordRequest request, Contexts contexts)
        {
            Log.Information("Started saving the message");
            var command = new AddMessageCommand(request.OriginalMessage,
                contexts.User.Id, contexts.User.Name,
                contexts.Channel.Id, contexts.Channel.Name,
                contexts.Server.Id, contexts.Server.Name,
                request.SentAt);
            Log.Information("Command created");
            await this._commandBus.ExecuteAsync(command);
            Log.Information("Message saved");
        }

        [AdminCommand]
        public async Task GetStatisticsPerPeriod(StatsCommand command, Contexts contexts)
        {
            if (command.Hour) // TODO get limits from configuration
            {
                await this.statisticsGenerator.PerHour(contexts.Server.Id, TimeSpan.FromDays(7)); 
            }
            else if (command.Day)
            {
                await this.statisticsGenerator.PerDay(contexts.Server.Id, TimeSpan.FromDays(30));
            }
            else if (command.Week)
            {
                await this.statisticsGenerator.PerWeek(contexts.Server.Id, TimeSpan.FromDays(90));
            }
            else if (command.Month)
            {
                await this.statisticsGenerator.PerMonth(contexts.Server.Id, TimeSpan.FromDays(365));
            }
            else if (command.Quarter)
            {
                await this.statisticsGenerator.PerQuarter(contexts.Server.Id, TimeSpan.FromDays(1825)); //5 years
            }


//            var period = this._reportsService.SelectPeriod(statsCommand);
//            var query = new GetMessagesStatisticsQuery(period);
//            var result = await this._queryBus.ExecuteAsync(query);
//            var periodStats = result.PeriodStatistics.Where(x => x.Count > 0);

//            var getMessages = new GetMessagesQuery(contexts.Server.Id);
//            var messages = this._queryBus.Execute(getMessages).Messages.ToList();
//            var report = this._reportsService.CreateReport(messages, period);
//            Log.Information("Generated statistics for time range {start} {end}", report.TimeRange.Start, report.TimeRange.End);
//#if DEBUG
//            PrintDebugStats(report);
//#endif
//            var stream = await this._chartsService.GetImageStatisticsPerPeriod(report);
//            var messagesService = this._messagesServiceFactory.Create(contexts);
//            await messagesService.SendFile("Statistics.png", stream);
        }

        private static void PrintDebugStats(StatisticsReport report)
        {
            var dataToMessage = JsonConvert.SerializeObject(report.StatisticsPerPeriod.Where(x => x.MessagesQuantity > 0), Formatting.Indented);
            var builder = new StringBuilder(dataToMessage).FormatMessageIntoBlock("json");
            Log.Information(builder.ToString());
        }
    }
}
