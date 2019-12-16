using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Watchman.DomainModel.Protection;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class AntiSpamService : IService
    {
        private readonly List<(ulong AuthorId, DateTime MessageDateTime)> _lastMessages = new List<(ulong AuthorId, DateTime MessageDateTime)>();
        private readonly List<ulong> _warns = new List<ulong>();

        public void AddUserMessage(Contexts contexts)
        {
            _lastMessages.Add((contexts.User.Id, DateTime.Now));
        }

        public void ClearOldMessages(int secondsAgo)
        {
            _lastMessages.RemoveAll(x => x.MessageDateTime < DateTime.Now.AddSeconds(-secondsAgo));
        }

        public int CountUserMessages(Contexts contexts, int seconds)
        {
            return _lastMessages
                .Where(x => x.MessageDateTime > DateTime.Now.AddSeconds(-seconds))
                .Count(x => x.AuthorId == contexts.User.Id);
        }

        public bool IsWarned(Contexts contexts)
        {
            return _warns.Contains(contexts.User.Id);
        }

        public void SetPunishment(Contexts contexts, MessagesService messagesService, ProtectionPunishment punishment)
        {
            switch (punishment.Option)
            {
                case ProtectionPunishmentOption.Clear:
                    _warns.Remove(contexts.User.Id);
                    break;

                case ProtectionPunishmentOption.Alert:
                    _warns.Add(contexts.User.Id);
                    messagesService.SendMessage($"Spam alert! Wykryto spam u użytkownika {contexts.User.Name} na kanale {contexts.User.Name}. Poczekaj chwile zanim coś napiszesz.").Wait();
                    break;

                case ProtectionPunishmentOption.Mute:
                    messagesService.SendMessage($"Spam alert! Uzytkownik {contexts.User.Name} został zmutowany.").Wait();
                    break;
            }
        }
    }
}
