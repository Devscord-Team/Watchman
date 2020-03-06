using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Discord.Areas.Protection.Services;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        private readonly AntiSpamService _antiSpamService;
        private readonly SpamDetectingStrategy _strategy;

        public AntiSpamController(AntiSpamService antiSpamService)
        {
            this._antiSpamService = antiSpamService;
            this._strategy = new SpamDetectingStrategy();
        }

        [ReadAlways]
        public void Scan(DiscordRequest request, Contexts contexts)
        {
            this._antiSpamService.AddUserMessage(contexts);
            var messagesInShortTime = _antiSpamService.CountUserMessagesShorterTime(contexts.User.Id);
            var messagesInLongTime = _antiSpamService.CountUserMessagesLongerTime(contexts.User.Id);
            var userWarnsInLastFewMinutes = _antiSpamService.CountUserWarnsInShortTime(contexts.User.Id);
            var userWarnsInLastFewHours = _antiSpamService.CountUserWarnsInLongTime(contexts.User.Id);

            var punishment = _strategy.SelectPunishment(userWarnsInLastFewMinutes, userWarnsInLastFewHours, messagesInShortTime, messagesInLongTime);
            _antiSpamService.SetPunishment(contexts, punishment);
        }
    }
}
