using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Devscord.DiscordFramework.Commands.AntiSpam.Models;
using Watchman.DomainModel.Muting;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Muting.Services.Commands;

namespace Watchman.Discord.Areas.Muting.Services
{
    public interface IAntiSpamService
    {
        Task SetPunishment(Contexts contexts, Punishment punishment);
    }

    public class AntiSpamService : IAntiSpamService
    {
        private readonly ICommandBus commandBus;
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly IUnmutingService _unmutingService;

        public AntiSpamService(ICommandBus commandBus, IMessagesServiceFactory messagesServiceFactory, IUnmutingService unmutingService)
        {
            this.commandBus = commandBus;
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
            await this.commandBus.ExecuteAsync(new MuteUserOrOverwriteCommand(contexts, muteEvent, contexts.User));
            this._unmutingService.UnmuteInFuture(contexts, muteEvent, contexts.User);
        }
    }
}
