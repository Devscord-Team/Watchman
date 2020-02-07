using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Mute;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly AntiSpamService _antiSpamService;
        private readonly SpamDetectingStrategy _strategy;

        public AntiSpamController(MessagesServiceFactory messagesServiceFactory, AntiSpamService antiSpamService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._antiSpamService = antiSpamService;
            this._strategy = new SpamDetectingStrategy();
        }

        [ReadAlways]
        public void Scan(DiscordRequest request, Contexts contexts)
        {
            var messagesService = _messagesServiceFactory.Create(contexts);
            _antiSpamService.ClearOldMessages(10);
            var messagesInLastTime = _antiSpamService.CountUserMessages(contexts, 10);
            var userIsWarned = _antiSpamService.IsWarned(contexts);
            var punishment = _strategy.SelectPunishment(userIsWarned, messagesInLastTime, 0);
            _antiSpamService.SetPunishment(contexts, messagesService, punishment);
        }
    }
}
