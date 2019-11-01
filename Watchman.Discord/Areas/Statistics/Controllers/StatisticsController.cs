using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Statistics.Models;
using Watchman.Discord.Areas.Statistics.Services;
using Watchman.Discord.Framework;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Integrations.MongoDB;

namespace Watchman.Discord.Areas.Statistics.Controllers
{
    public class StatisticsController : IController
    {
        //TODO kontroler
        //zapisywanie wiadomości -> możliwe że w innym kontrolerze
        //generowanie statystyk jako excel, który idzie do google drive
        //generowanie statystyk jako ładny obraz .png, który jest pokazywany użytkownikom 
        //statystyki -> per kanał, per osoba, per serwer, wszystko z wybranego przedziału dat, domyślnie 30 dni

        //tool dla administracji 
        //mechanizm przewidujący kolejne miesiące i szukający zależności 
        //do tego trzeba dodać informacje o datach i miejscach reklamy
        //można to zrobić jako narzędzie w pythonie, bo tam jest dużo bibliotek do ML, 
        //ewentualnie sprawdzić jak radzi sobie ML.NET


        private readonly ISession _session;
        private readonly ReportsService _reportsService;
        private readonly ChartsService _chartsService;

        public StatisticsController()
        {
            this._session = new SessionFactory(Server.GetDatabase()).Create(); //todo use IoC
            this._reportsService = new ReportsService();
            this._chartsService = new ChartsService();
        }

        //TODO informations about author, channel and server should be set in middleware 
        //so that MessageContext or UserContext & ChannelContext & ServerContext objects should be added

        [ReadAlways]
        public void SaveMessage(SocketMessage message)
        {
            //todo wrzucić budowanie tego obiektu w osobną klase -> MessageInformationBuilder

            //probably should be structures, not classes
            var author = new MessageInformationAuthor
            {
                Id = message.Author.Id,
                Name = message.Author.ToString() //TODO save more values, for example Discriminator
            };
            var channel = new MessageInformationChannel
            {
                Id = message.Channel.Id,
                Name = message.Channel.Name
            };


            var serverInfo = ((SocketGuildChannel)message.Channel).Guild;
            
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

            var content = message.Content;
            var date = DateTime.UtcNow;

            var result = new MessageInformation
            {
                Author = author,
                Channel = channel,
                Server = server,
                Content = content,
                Date = date
            };
#if DEBUG
            //uncomment for tests
            //if (channel.Name.ToLowerInvariant().Contains("test") && server.Name.ToLowerInvariant().Contains("test"))
            //{
            //    var dataToMessage = "```json\n" + JsonConvert.SerializeObject(result, Formatting.Indented) + "\n```";
            //    message.Channel.SendMessageAsync(dataToMessage);
            //}
#endif

            this.SaveToDatabase(result);
        }

        [DiscordCommand("-stats")]
        public void GetStatisticsPerPeriod(SocketMessage message)
        {

#if DEBUG
            if (!message.Channel.Name.Contains("test"))
            {
                return;
            }
#endif

            var period = Period.Day;
            //todo other class in Commons
            if (message.Content.ToLowerInvariant().Contains("hour"))
            {
                period = Period.Hour;
            }
            else if (message.Content.ToLowerInvariant().Contains("day"))
            {
                period = Period.Day;
            }
            else if (message.Content.ToLowerInvariant().Contains("week"))
            {
                period = Period.Week;
            }
            else if (message.Content.ToLowerInvariant().Contains("month"))
            {
                period = Period.Month;
            }

            //todo set oldest possible based on period
            var messages = this._session.Get<MessageInformation>().ToList();
            var report = _reportsService.CreateReport(messages, period);

#if DEBUG

            var dataToMessage = "```json\n" + JsonConvert.SerializeObject(report.StatisticsPerPeriod.Where(x => x.MessagesQuantity > 0), Formatting.Indented) + "\n```";
            message.Channel.SendMessageAsync(dataToMessage);
#endif
            var path = _chartsService.GetImageStatisticsPerPeriod(report);
            message.Channel.SendFileAsync(path);
        }

        private Task SaveToDatabase(MessageInformation data)
        {
            Task.Factory.StartNew(() => _session.Add(data));
            return Task.CompletedTask;
        }
    }
}
