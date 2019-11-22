using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AntiSpamController : IController
    {
        //TODO balans
        private List<(ulong AuthorId, DateTime MessageDateTime)> _lastMessages = new List<(ulong, DateTime)>();
        private List<ulong> _warns = new List<ulong>();

        public AntiSpamController()
        {
        }

        [ReadAlways]
        public void Scan(string message, Dictionary<string, IDiscordContext> contexts)
        {
            var userContext = (UserContext) contexts[nameof(UserContext)];

            _lastMessages.RemoveAll(x => x.MessageDateTime < DateTime.Now.AddSeconds(-10));

            var messagesInLastTime = _lastMessages.Where(x => x.AuthorId == userContext.Id).Count();
            if (messagesInLastTime >= 5)
            {
                var channelContext = (ChannelContext) contexts[nameof(ChannelContext)];
                var messagesService = new MessagesService { DefaultChannelId = channelContext.Id };
                if (!_warns.Contains(userContext.Id))
                {
                    _warns.Add(userContext.Id);
                    messagesService.SendMessage($"Spam alert! Wykryto spam u użytkownika {userContext.Name} na kanale {channelContext.Name}. Poczekaj chwile zanim coś napiszesz.").Wait();
                }
                else if (messagesInLastTime > 10)
                {
                    //todo add role "mute", and add service to deleting it automatically
                    messagesService.SendMessage($"Spam alert! Uzytkownik {userContext.Name} został zmutowany.").Wait();
                }
                
            }
            else if (_warns.Contains(userContext.Id) && !_lastMessages.Any(x => x.AuthorId == userContext.Id)) //todo optimalize
            {
                _warns.Remove(userContext.Id);
            }

            _lastMessages.Add((userContext.Id, DateTime.Now));
        }
    }
}
