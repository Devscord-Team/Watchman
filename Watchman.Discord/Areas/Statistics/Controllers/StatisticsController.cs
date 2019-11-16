using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Statistics.Models;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.Discord.Framework;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Statistics.Controllers
{
    public class StatisticsController : IController
    {
        private readonly ISession _session;
        private readonly ReportsService _reportsService;
        private readonly ChartsService _chartsService;

        public StatisticsController()
        {
            this._session = new SessionFactory(Server.GetDatabase()).Create(); //todo use IoC
            this._reportsService = new ReportsService();
            this._chartsService = new ChartsService();
        }

        [ReadAlways]
        public void SaveMessage(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var userContext = (UserContext) contexts[nameof(UserContext)];
            var channelContext = (ChannelContext) contexts[nameof(ChannelContext)];

            var author = new MessageInformationAuthor
            {
                Id = userContext.Id,
                Name = userContext.Name
            };
            var channel = new MessageInformationChannel
            {
                Id = channelContext.Id,
                Name = channelContext.Name
            };


            var serverInfo = ((SocketGuildChannel)Server.GetChannel(channelContext.Id)).Guild;
            
            var server = new MessageInformationServer
            {
                Id = serverInfo.Id,
                Name = serverInfo.Name,
                Owner = new MessageInformationAuthor
                {
                    Id = serverInfo.Owner.Id,
                    Name = serverInfo.Owner.ToString()
                }
            };

            var date = DateTime.UtcNow;

            var result = new MessageInformation
            {
                Author = author,
                Channel = channel,
                Server = server,
                Content = message,
                Date = date
            };

            this.SaveToDatabase(result);
        }

        [AdminCommand]
        [DiscordCommand("-stats")]
        public void GetStatisticsPerPeriod(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var period = Period.Day;
            //todo other class in Commons
            if (message.ToLowerInvariant().Contains("hour"))
            {
                period = Period.Hour;
            }
            else if (message.ToLowerInvariant().Contains("day"))
            {
                period = Period.Day;
            }
            else if (message.ToLowerInvariant().Contains("week"))
            {
                period = Period.Week;
            }
            else if (message.ToLowerInvariant().Contains("month"))
            {
                period = Period.Month;
            }

            //todo set oldest possible based on period
            var messages = this._session.Get<MessageInformation>().ToList();
            var report = _reportsService.CreateReport(messages, period);

            var channelContext = (ChannelContext) contexts[nameof(ChannelContext)];
            var channel = (ISocketMessageChannel) Server.GetChannel(channelContext.Id);
#if DEBUG

            var dataToMessage = "```json\n" + JsonConvert.SerializeObject(report.StatisticsPerPeriod.Where(x => x.MessagesQuantity > 0), Formatting.Indented) + "\n```";
            channel.SendMessageAsync(dataToMessage);
#endif
            var path = _chartsService.GetImageStatisticsPerPeriod(report);
            channel.SendFileAsync(path);
        }

        private Task SaveToDatabase(MessageInformation data)
        {
            Task.Factory.StartNew(() => _session.Add(data));
            return Task.CompletedTask;
        }
    }
}
