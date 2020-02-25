using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using Watchman.DomainModel.Users;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class AntiSpamService
    {
        private readonly MuteService _muteService;

        public AntiSpamService(MuteService muteService)
        {
            _muteService = muteService;
        }

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
            if(punishment.Option != ProtectionPunishmentOption.Clear && punishment.Option != ProtectionPunishmentOption.Nothing)
            {
                Log.Information("Spam recognized! User: {user} on channel: {channel} server: {server}",
                contexts.User.Name, contexts.Channel.Name, contexts.Server.Name);
            }
            switch (punishment.Option)
            {
                case ProtectionPunishmentOption.Clear:
                    _warns.Remove(contexts.User.Id);
                    break;

                case ProtectionPunishmentOption.Alert:
                    _warns.Add(contexts.User.Id);
                    messagesService.SendResponse(x => x.SpamAlertRecognized(contexts), contexts);
                    break;

                case ProtectionPunishmentOption.Mute:
                    //todo:auto muting
                    messagesService.SendResponse(x => x.SpamAlertUserIsMuted(contexts), contexts);
                    break;
            }
        }
    }
}
