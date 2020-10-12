using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Watchman.DomainModel.Users;
using Devscord.DiscordFramework.Framework.Commands.AntiSpam.Models;
using System.Text;
using Devscord.DiscordFramework.Services;
using Watchman.Discord.Areas.Users.Services;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class AntiSpamService
    {
        private readonly MutingService _mutingService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UnmutingService _unmutingService;
        private readonly WarnsService _warnsService;
        private readonly UsersService _usersService;
        private readonly ResponsesService _responsesService;

        public AntiSpamService(MutingService mutingService, MessagesServiceFactory messagesServiceFactory, UnmutingService unmutingService, WarnsService warnsService, UsersService usersService, ResponsesService responsesService)
        {
            this._mutingService = mutingService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._unmutingService = unmutingService;
            this._warnsService = warnsService;
            this._usersService = usersService;
            this._responsesService = responsesService;
        }

        public async Task SetPunishment(Contexts contexts, Punishment punishment)
        {
            if (punishment.PunishmentOption == PunishmentOption.Nothing)
            {
                return;
            }
            Log.Information("Spam recognized! User: {user} on channel: {channel} server: {server}", contexts.User.Name, contexts.Channel.Name, contexts.Server.Name);
            var messagesService = this._messagesServiceFactory.Create(contexts);
            var warnReason = new StringBuilder().Append($"Ostrzeżenie za spam na kanale: #{contexts.Channel.Name}");
            switch (punishment.PunishmentOption)
            {
                case PunishmentOption.Warn:
                    await messagesService.SendResponse(x => x.SpamAlertRecognized(contexts));
                    break;
                case PunishmentOption.Mute:
                    warnReason.Append($" - wyciszono na {punishment.ForTime!.Value}");
                    await this.MuteUserForSpam(contexts, punishment.ForTime!.Value);
                    break;
            }
            await this._warnsService.AddWarnToUser(_usersService.GetBot().Id, contexts.User.Id, warnReason.ToString(), contexts.Server.Id);
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
