using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Services;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;
        private readonly MessagesServiceFactory messagesServiceFactory;
        private readonly AntiSpamDomainStrategy _strategy;

        //TODO balans
        private readonly List<(ulong AuthorId, DateTime MessageDateTime)> _lastMessages;
        private readonly List<ulong> _warns;

        public AntiSpamController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
            this.messagesServiceFactory = messagesServiceFactory;
            this._strategy = new AntiSpamDomainStrategy();
            
            this._lastMessages = new List<(ulong, DateTime)>();
            this._warns = new List<ulong>();
        }

        [ReadAlways]
        public void Scan(string message, Contexts contexts)
        {
            var messagesService = messagesServiceFactory.Create(contexts);

            _lastMessages.RemoveAll(x => x.MessageDateTime < DateTime.Now.AddSeconds(-10));

            var messagesInLastTime = _lastMessages.Count(x => x.AuthorId == contexts.User.Id);
            var punishment = _strategy.SelectPunishment(_warns.Contains(contexts.User.Id), messagesInLastTime, 0);

            switch (punishment.Option)
            {
                case DomainModel.Protection.ProtectionPunishmentOptions.Clear:
                        _warns.Remove(contexts.User.Id);
                    break;

                case DomainModel.Protection.ProtectionPunishmentOptions.Alert:
                    _warns.Add(contexts.User.Id);
                    messagesService.SendMessage($"Spam alert! Wykryto spam u użytkownika {contexts.User.Name} na kanale {contexts.User.Name}. Poczekaj chwile zanim coś napiszesz.").Wait();
                    break;

                case DomainModel.Protection.ProtectionPunishmentOptions.Mute:
                    messagesService.SendMessage($"Spam alert! Uzytkownik {contexts.User.Name} został zmutowany.").Wait();
                    break;
            }

            _lastMessages.Add((contexts.User.Id, DateTime.Now));
        }
    }
}
