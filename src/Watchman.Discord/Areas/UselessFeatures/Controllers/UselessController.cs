﻿using System;
using System.Linq;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System.Threading.Tasks;
using Watchman.Discord.Areas.UselessFeatures.BotCommands;
using Watchman.Integrations.Images;

namespace Watchman.Discord.Areas.UselessFeatures.Controllers
{
    public class UselessController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly ImagesService _imagesService;

        public UselessController(MessagesServiceFactory messagesServiceFactory, ImagesService imageService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._imagesService = imageService;
        }

        public async Task PrintMarchew(MarchewCommand command, Contexts contexts)
        {
            const string text = "Moim zdaniem to nie ma tak, że coś jest programowaniem, albo nie jest programowaniem. Gdybym miał powiedzieć, co cenię w programowaniu najbardziej, powiedziałbym, że ludzi. Ekhm… Ludzi, którzy podali mi pomocną dokumentacje, kiedy sobie nie radziłem, kiedy byłem sam. I co ciekawe, to właśnie przypadkowe spotkania wpływają na nasze życie. Chodzi o to, że kiedy wyznaje się pewne wartości, nawet pozornie uniwersalne, bywa, że nie znajduje się zrozumienia, które by tak rzec, które pomaga się nam rozwijać. Ja miałem szczęście, by tak rzec, ponieważ je znalazłem. I dziękuję życiu. Dziękuję mu, życie to śpiew, życie to taniec, życie to miłość. Wielu ludzi pyta mnie o to samo, ale jak ty to robisz? Skąd czerpiesz tę radość? A ja odpowiadam, że to proste, to umiłowanie życia, to właśnie ono sprawia, że dzisiaj na przykład programuje nawigacje, a jutro… kto wie, dlaczego by nie, oddam się pracy społecznej i będę ot, choćby uczyć… znaczy… juniorów.";
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendMessage(text);
        }

        public async Task SendMarudaImage(MarudaCommand command, Contexts contexts)
        {
            var allImages = this._imagesService.GetImagesFromResources(x => x.StartsWith("maruda"));
            var randomImage = allImages.ElementAt(new Random().Next(allImages.Count()));
            await this._messagesServiceFactory.Create(contexts).SendFile(randomImage.Name, randomImage.Stream);
        }
    }
}
