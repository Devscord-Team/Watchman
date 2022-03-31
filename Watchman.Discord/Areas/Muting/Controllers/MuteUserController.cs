using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Muting.Services;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Muting.Services.Commands;
using Watchman.Discord.Areas.Muting.BotCommands;
using Watchman.DomainModel.Muting;

namespace Watchman.Discord.Areas.Muting.Controllers
{
    public class MuteUserController : IController
    {
        private readonly ICommandBus commandBus;
        private readonly IUnmutingService _unmutingService;
        private readonly IUsersService _usersService;
        private readonly IMessagesServiceFactory _messagesServiceFactory;

        public MuteUserController(ICommandBus commandBus, IUnmutingService unmutingService, IUsersService usersService, IMessagesServiceFactory messagesServiceFactory)
        {
            this.commandBus = commandBus;
            this._unmutingService = unmutingService;
            this._usersService = usersService;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        public async Task MuteUser(MuteCommand command, Contexts contexts)
        {
            var userToMute = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            if (userToMute == null || userToMute.Id == this._usersService.GetBot().Id)
            {
                throw new UserNotFoundException(command.User.GetUserMention());
            }
            var timeRange = TimeRange.FromNow(DateTime.UtcNow + command.Time); //todo: change DateTime.UtcNow to Contexts.SentAt
            var muteEvent = new MuteEvent(userToMute.Id, timeRange, command.Reason, contexts.Server.Id, contexts.Channel.Id);
            await this.commandBus.ExecuteAsync(new MuteUserOrOverwriteCommand(contexts, muteEvent, userToMute));
            this._unmutingService.UnmuteInFuture(contexts, muteEvent, userToMute);
        }

        [AdminCommand]
        public async Task UnmuteUser(UnmuteCommand command, Contexts contexts)
        {
            var userToUnmute = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            if (userToUnmute == null)
            {
                throw new UserNotFoundException(command.User.GetUserMention());
            }
            await this.commandBus.ExecuteAsync(new UnmuteNowCommand(contexts, userToUnmute));
        }

        [AdminCommand]
        public async Task GetMutedUsers(MutedUsersCommand mutedUsersCommand, Contexts contexts)
        {
            await this.commandBus.ExecuteAsync(new SendMutedUsersDirectMessageCommand(contexts));
            var messagesService = this._messagesServiceFactory.Create(contexts);
            await messagesService.SendResponse(x => x.MutedUsersListSent());
        }
    }
}
