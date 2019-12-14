using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
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
        private readonly AntiSpamDomainStrategy _strategy;

        //TODO balans
        private readonly List<(ulong AuthorId, DateTime MessageDateTime)> _lastMessages;
        private readonly List<ulong> _warns;

        public AntiSpamController(IQueryBus queryBus, ICommandBus commandBus)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
            this._strategy = new AntiSpamDomainStrategy();
            
            this._lastMessages = new List<(ulong, DateTime)>();
            this._warns = new List<ulong>();
        }

        [ReadAlways]
        public void Scan(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var userContext = (UserContext) contexts[nameof(UserContext)];
            var channelContext = (ChannelContext)contexts[nameof(ChannelContext)];
            var messagesService = new MessagesService { DefaultChannelId = channelContext.Id };

            _lastMessages.RemoveAll(x => x.MessageDateTime < DateTime.Now.AddSeconds(-10));

            var messagesInLastTime = _lastMessages.Count(x => x.AuthorId == userContext.Id);
            var punishment = _strategy.SelectPunishment(_warns.Contains(userContext.Id), messagesInLastTime, 0);

            switch (punishment.Option)
            {
                case DomainModel.Protection.ProtectionPunishmentOptions.Clear:
                        _warns.Remove(userContext.Id);
                    break;

                case DomainModel.Protection.ProtectionPunishmentOptions.Alert:
                    _warns.Add(userContext.Id);
                    messagesService.SendMessage($"Spam alert! Wykryto spam u użytkownika {userContext.Name} na kanale {channelContext.Name}. Poczekaj chwile zanim coś napiszesz.").Wait();
                    break;

                case DomainModel.Protection.ProtectionPunishmentOptions.Mute:
                    messagesService.SendMessage($"Spam alert! Uzytkownik {userContext.Name} został zmutowany.").Wait();
                    break;
            }

            _lastMessages.Add((userContext.Id, DateTime.Now));
        }
    }
}
