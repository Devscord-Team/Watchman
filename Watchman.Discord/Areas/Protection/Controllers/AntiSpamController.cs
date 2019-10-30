using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.Discord.Framework.Architecture.Controllers;

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
        public void Scan(SocketMessage message)
        {
            var authorId = message.Author.Id;

            var messagesInLastTime = _lastMessages.Where(x => x.MessageDateTime >= DateTime.Now.AddSeconds(-10) && x.AuthorId == authorId).Count();
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
            else if (_warns.Contains(authorId) && !_lastMessages.Any(x => x.MessageDateTime >= DateTime.Now.AddSeconds(-10) && x.AuthorId == authorId)) //todo optimalize
            {
                _warns.Remove(authorId);
            }
            _lastMessages.Add((authorId, DateTime.Now));
            _lastMessages.Where(x => x.MessageDateTime < DateTime.Now.AddSeconds(-15))
                .ToList()
                .ForEach(x => _lastMessages.Remove(x));
        }
    }
}
