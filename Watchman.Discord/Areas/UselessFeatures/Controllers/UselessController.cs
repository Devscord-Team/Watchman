using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.UselessFeatures.BotCommands;

namespace Watchman.Discord.Areas.UselessFeatures.Controllers
{
    public class UselessController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public UselessController(MessagesServiceFactory messagesServiceFactory)
        {
            _messagesServiceFactory = messagesServiceFactory;
        }

        [DiscordCommand("marchew")]
        public async Task PrintMarchew(DiscordRequest request, Contexts contexts)
        {
            const string text = "Moim zdaniem to nie ma tak, że coś jest programowaniem, albo nie jest programowaniem. Gdybym miał powiedzieć, co cenię w programowaniu najbardziej, powiedziałbym, że ludzi. Ekhm… Ludzi, którzy podali mi pomocną dokumentacje, kiedy sobie nie radziłem, kiedy byłem sam. I co ciekawe, to właśnie przypadkowe spotkania wpływają na nasze życie. Chodzi o to, że kiedy wyznaje się pewne wartości, nawet pozornie uniwersalne, bywa, że nie znajduje się zrozumienia, które by tak rzec, które pomaga się nam rozwijać. Ja miałem szczęście, by tak rzec, ponieważ je znalazłem. I dziękuję życiu. Dziękuję mu, życie to śpiew, życie to taniec, życie to miłość. Wielu ludzi pyta mnie o to samo, ale jak ty to robisz? Skąd czerpiesz tę radość? A ja odpowiadam, że to proste, to umiłowanie życia, to właśnie ono sprawia, że dzisiaj na przykład programuje nawigacje, a jutro… kto wie, dlaczego by nie, oddam się pracy społecznej i będę ot, choćby uczyć… znaczy… juniorów.";
            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendMessage(text);
        }

        [DiscordCommand("embed")]
        public async Task Embed(DiscordRequest request, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);
            await messagesService.SendEmbedMessage("Testowy tytuł", "Testowy opis", new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("Test1", "Opis1"),
                new KeyValuePair<string, string>("Inny tytuł", "Inny opis"),
                new KeyValuePair<string, string>("Testowe dane", "Troche inny opis, taki dłuższy \nzawierający enter!"),
            });
        }

        public async Task PrintMyMessage(PrintCommand command, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);
            for (int i = 0; i < command.Times; i++)
            {
                await messagesService.SendMessage(command.Message);
            }
        }
    }
}
