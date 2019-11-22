using Devscord.DiscordFramework.Framework;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Architecture.Middlewares;
using Devscord.DiscordFramework.Middlewares.Contexts;
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

            var author = Server.GetUser(userContext.Id);
            var authorId = author.Id;

            _lastMessages.RemoveAll(x => x.MessageDateTime < DateTime.Now.AddSeconds(-10));

            var messagesInLastTime = _lastMessages.Where(x => x.AuthorId == authorId).Count();
            if (messagesInLastTime >= 5)
            {
                var channelContext = (ChannelContext) contexts[nameof(ChannelContext)];
                var channel = (ISocketMessageChannel) Server.GetChannel(channelContext.Id);

                if (!_warns.Contains(authorId))
                {
                    _warns.Add(authorId);
                    channel.SendMessageAsync($"Spam alert! Wykryto spam u użytkownika {author} na kanale {channel.Name}. Poczekaj chwile zanim coś napiszesz.").Wait();
                }
                else if (messagesInLastTime > 10)
                {
                    //todo add role "mute", and add service to deleting it automatically
                    channel.SendMessageAsync($"Spam alert! Uzytkownik {author} został zmutowany.").Wait();
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
