using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Watchman.DomainModel.Users;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class AntiSpamService
    {
        private readonly MuteService _muteService;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public AntiSpamService(MuteService muteService, MessagesServiceFactory messagesServiceFactory)
        {
            this._muteService = muteService;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public async Task SetPunishment(Contexts contexts, Punishment punishment)
        {
            if (punishment.PunishmentOption == PunishmentOption.Nothing)
            {
                return;
            }

            Log.Information("Spam recognized! User: {user} on channel: {channel} server: {server}",
                            contexts.User.Name, contexts.Channel.Name, contexts.Server.Name);

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
            var timeRange = new TimeRange(DateTime.Now, DateTime.Now.Add(length));
            var muteEvent = new MuteEvent(contexts.User.Id, timeRange, "Spam detected (by bot)", contexts.Server.Id);
            await this._muteService.MuteUserOrOverwrite(contexts, muteEvent, contexts.User);
            this._muteService.UnmuteInFuture(contexts, muteEvent, contexts.User);
        }
    }
}
