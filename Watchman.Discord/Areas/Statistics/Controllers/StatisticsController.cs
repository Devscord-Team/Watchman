using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Newtonsoft.Json;
using Serilog;
using Statsman;
using Statsman.Models;
using System;
using System.Collections.Generic;
using System.IO;
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
        
        private readonly StatisticsGenerator statisticsGenerator;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public StatisticsController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, ChartsService chartsService, StatisticsGenerator statisticsGenerator)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
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
            var (chart, message) = await this.GetStatistics(command, contexts);
            if (chart == null || string.IsNullOrWhiteSpace(message))
            {
                return;
            }
            var messagesService = this._messagesServiceFactory.Create(contexts);
            _ = await Task.Run(() => messagesService.SendFile("Statistics.png", chart))
                .ContinueWith(x => messagesService.SendMessage(message));
        }

        private Task<(Stream Chart, string Message)> GetStatistics(StatsCommand command, Contexts contexts)
        {
            if (command.Minute)
            {
                return this.statisticsGenerator.PerMinute(contexts.Server.Id, TimeSpan.FromMinutes(60)); // TODO get time limits from configuration
            }
            else if (command.Hour)
            {
                return this.statisticsGenerator.PerHour(contexts.Server.Id, TimeSpan.FromDays(7)); 
            }
            else if (command.Day)
            {
                return this.statisticsGenerator.PerDay(contexts.Server.Id, TimeSpan.FromDays(30));
            }
            else if (command.Week)
            {
                return this.statisticsGenerator.PerWeek(contexts.Server.Id, TimeSpan.FromDays(90));
            }
            else if (command.Month)
            {
                return this.statisticsGenerator.PerMonth(contexts.Server.Id, TimeSpan.FromDays(365));
            }
            else if (command.Quarter)
            {
                return this.statisticsGenerator.PerQuarter(contexts.Server.Id, TimeSpan.FromDays(1825)); //5 years
            }
            return null;
        }
    }
    
}
