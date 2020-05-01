using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Responses.Controllers
{
    public class ResponsesController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly UsersRolesService _usersRolesService;
        private readonly Services.ResponsesService _responsesService;

        public ResponsesController(IQueryBus queryBus, ICommandBus commandBus, UsersService usersService, DirectMessagesService directMessagesService, MessagesServiceFactory messagesServiceFactory, UsersRolesService usersRolesService, Services.ResponsesService responsesService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._responsesService = responsesService;
            _usersRolesService = usersRolesService;
        }

        [AdminCommand]
        [DiscordCommand("add response")]
        public async Task AddResponse(DiscordRequest request, Contexts contexts)
        {
            var messageService = _messagesServiceFactory.Create(contexts);
            var onEvent = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "onevent")?.Value;
            var message = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "message")?.Value;

            if (onEvent == null || message == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments(), contexts);
                return;
            }

            var response = await _responsesService.GetResponseByOnEvent(onEvent);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, onEvent), contexts);
                return;
            }

            var responseForThisServer = await _responsesService.GetResponseByOnEvent(onEvent, contexts.Server.Id);
            if (responseForThisServer != null)
            {
                await messageService.SendResponse(x => x.ResponseAlreadyExists(contexts, onEvent), contexts);
                return;
            }

            await _responsesService.AddResponse(onEvent, message, contexts.Server.Id);
            await messageService.SendResponse(x => x.ResponseHasBeenAdded(contexts, onEvent), contexts);
        }

        [AdminCommand]
        [DiscordCommand("update response")]
        public async Task UpdateResponse(DiscordRequest request, Contexts contexts)
        {
            var messageService = _messagesServiceFactory.Create(contexts);
            var onEvent = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "onevent")?.Value;
            var message = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "message")?.Value;

            if (onEvent == null || message == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments(), contexts);
                return;
            }

            var response = await _responsesService.GetResponseByOnEvent(onEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, onEvent), contexts);
                return;
            }

            await _responsesService.UpdateResponse(response.Id, message);
            await messageService.SendResponse(x => x.ResponseHasBeenUpdated(contexts, onEvent, response.Message, message), contexts);
        }

        [AdminCommand]
        [DiscordCommand("remove response")]
        public async Task RemoveResponse(DiscordRequest request, Contexts contexts)
        {
            var messageService = _messagesServiceFactory.Create(contexts);
            var onEvent = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "onevent")?.Value;

            if (onEvent == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments(), contexts);
                return;
            }

            var response = await _responsesService.GetResponseByOnEvent(onEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, onEvent), contexts);
                return;
            }

            await _responsesService.RemoveResponse(onEvent, contexts.Server.Id);
            await messageService.SendResponse(x => x.ResponseHasBeenRemoved(contexts, onEvent), contexts);
        }
    }
}
