using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Linq;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;
        private readonly MessagesServiceFactory messagesServiceFactory;
        private readonly AntiSpamService antiSpamService;
        private readonly AntiSpamDomainStrategy _strategy;

        public AntiSpamController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, AntiSpamService antiSpamService)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
            this.messagesServiceFactory = messagesServiceFactory;
            this.antiSpamService = antiSpamService;
            this._strategy = new AntiSpamDomainStrategy();
        }

        [ReadAlways]
        public void Scan(string message, Contexts contexts)
        {
            var messagesService = messagesServiceFactory.Create(contexts);
            antiSpamService.ClearOldMessages(10);
            var messagesInLastTime = antiSpamService.CountUserMessages(contexts, 10);
            var userIsWarned = antiSpamService.IsWarned(contexts);
            var punishment = _strategy.SelectPunishment(userIsWarned, messagesInLastTime, 0);
            antiSpamService.SetPunishment(contexts, messagesService, punishment);
        }
    }
}
