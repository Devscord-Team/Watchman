using System;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Discord.WebSocket;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.DomainModel.Messages.Commands;
using Watchman.DomainModel.Messages.Queries;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Statistics.Controllers
{
    public class StatisticsController : IController
    {
        private readonly ReportsService _reportsService;
        private readonly ChartsService _chartsService;
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;
        private readonly MessagesServiceFactory messagesServiceFactory;

        public StatisticsController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, ReportsService reportsService, ChartsService chartsService)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
            this.messagesServiceFactory = messagesServiceFactory;
            this._reportsService = reportsService;
            this._chartsService = chartsService;
        }

        [ReadAlways]
        public async void SaveMessageAsync(DiscordRequest request, Contexts contexts)
        {
            //TODO maybe there should be builder... but it doesn't looks very bad
            var command = new AddMessageCommand(request.OriginalMessage,
                contexts.User.Id, contexts.User.Name,
                contexts.Channel.Id, contexts.Channel.Name,
                contexts.Server.Id, contexts.Server.Name,
                contexts.Server.Owner.Id, contexts.Server.Owner.Name,
                DateTime.UtcNow);

            await this.commandBus.ExecuteAsync(command);
            Log.Information("Message saved");
        }

        [AdminCommand]
        [DiscordCommand("stats")]
        public void GetStatisticsPerPeriod(DiscordRequest request, Contexts contexts)
        {
            //TODO it doesn't looks clear...
            var period = _reportsService.SelectPeriod(request.OriginalMessage); //TODO use DiscordRequest properties
            var messages = this.queryBus.Execute(new GetMessagesQuery()).Messages;
            var report = _reportsService.CreateReport(messages, period, contexts.Server);
            Log.Information("Generated statistics for time range {start} {end}", report.TimeRange.Start, report.TimeRange.End);
            var messagesService = messagesServiceFactory.Create(contexts);
#if DEBUG
            //TODO it should be inside messages service... or responses service
            var dataToMessage = "```json\n" + JsonConvert.SerializeObject(report.StatisticsPerPeriod.Where(x => x.MessagesQuantity > 0), Formatting.Indented) + "\n```";
            messagesService.SendMessage(dataToMessage);
#endif
            var path = _chartsService.GetImageStatisticsPerPeriod(report);
            messagesService.SendFile(path);
        }
    }
}
