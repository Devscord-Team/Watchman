﻿using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using Watchman.DomainModel.Protection.Mutes;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class AntiSpamService
    {
        private readonly MutingService _mutingService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UnmutingService _unmutingService;

        public AntiSpamService(MutingService mutingService, MessagesServiceFactory messagesServiceFactory, UnmutingService unmutingService)
        {
            this._mutingService = mutingService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._unmutingService = unmutingService;
        }

        public async Task SetPunishment(Contexts contexts, Punishment punishment)
        {
            if (punishment.PunishmentOption == PunishmentOption.Nothing)
            {
                return;
            }
            Log.Information("Spam recognized! User: {user} on channel: {channel} server: {server}", contexts.User.Name, contexts.Channel.Name, contexts.Server.Name);

            var messagesService = this._messagesServiceFactory.Create(contexts);
            switch (punishment.PunishmentOption)
            {
                case PunishmentOption.Warn:
                    await messagesService.SendResponse(x => x.SpamAlertRecognized(contexts));
                    break;
                case PunishmentOption.Mute:
                    await this.MuteUserForSpam(contexts, punishment.ForTime!.Value);
                    break;
            }
        }

        private async Task MuteUserForSpam(Contexts contexts, TimeSpan length)
        {
            var timeRange = new TimeRange(DateTime.UtcNow, DateTime.UtcNow.Add(length));
            var muteEvent = new MuteEvent(contexts.User.Id, timeRange, "Spam detected (by bot)", contexts.Server.Id, contexts.Channel.Id);
            await this._mutingService.MuteUserOrOverwrite(contexts, muteEvent, contexts.User);
            this._unmutingService.UnmuteInFuture(contexts, muteEvent, contexts.User);
        }
    }
}
