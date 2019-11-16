using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.Discord.Framework.Architecture.Controllers;
using Watchman.Discord.Framework.Architecture.Middlewares;

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
            var authorId = message.Author.Id;

            _lastMessages.RemoveAll(x => x.MessageDateTime < DateTime.Now.AddSeconds(-10));

            var messagesInLastTime = _lastMessages.Where(x => x.AuthorId == authorId).Count();
            if (messagesInLastTime >= 5)
            {
                if (!_warns.Contains(authorId))
                {
                    _warns.Add(authorId);
                    message.Channel.SendMessageAsync($"Spam alert! Wykryto spam u użytkownika {message.Author} na kanale {message.Channel.Name}. Poczekaj chwile zanim coś napiszesz.").Wait();
                }
                else if (messagesInLastTime > 10)
                {
                    //todo add role "mute", and add service to deleting it automatically
                    message.Channel.SendMessageAsync($"Spam alert! Uzytkownik {message.Author} został zmutowany.").Wait();
                }
                
            }
            else if (_warns.Contains(authorId) && !_lastMessages.Any(x => x.AuthorId == authorId)) //todo optimalize
            {
                _warns.Remove(authorId);
            }

            _lastMessages.Add((authorId, DateTime.Now));
        }
    }
}
