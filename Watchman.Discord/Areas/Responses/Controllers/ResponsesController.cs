<<<<<<< HEAD
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
=======
﻿using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
>>>>>>> master
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
<<<<<<< HEAD
using System;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;
=======
>>>>>>> master
using Watchman.Discord.Areas.Responses.Services;

namespace Watchman.Discord.Areas.Responses.Controllers
{
    public class ResponsesController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly Services.ResponsesService _responsesService;
        private readonly ResponsesMessageService _responsesMessageService;
        private readonly string[] _possibleArguments = { "all", "default", "custom" };

<<<<<<< HEAD
        public ResponsesController(IQueryBus queryBus, ICommandBus commandBus, UsersService usersService, DirectMessagesService directMessagesService, MessagesServiceFactory messagesServiceFactory,
            Services.ResponsesService responsesService, ResponsesMessageService responsesMessageService)
=======
        public ResponsesController(MessagesServiceFactory messagesServiceFactory, Services.ResponsesService responsesService, ResponsesMessageService responsesMessageService)
>>>>>>> master
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._responsesService = responsesService;
            this._responsesMessageService = responsesMessageService;
        }

        [AdminCommand]
        [DiscordCommand("add response")]
        public async Task AddResponse(DiscordRequest request, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var onEvent = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "onevent")?.Value;
            var message = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "message")?.Value;
            if (onEvent == null || message == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments());
                return;
            }
            var response = await this._responsesService.GetResponseByOnEvent(onEvent);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, onEvent));
                return;
            }
            var responseForThisServer = await this._responsesService.GetResponseByOnEvent(onEvent, contexts.Server.Id);
            if (responseForThisServer != null)
            {
                await messageService.SendResponse(x => x.ResponseAlreadyExists(contexts, onEvent));
                return;
            }
            await this._responsesService.AddResponse(onEvent, message, contexts.Server.Id);
<<<<<<< HEAD
            await messageService.SendResponse(x => x.ResponseHasBeenAdded(contexts, onEvent), contexts);
=======
            await messageService.SendResponse(x => x.ResponseHasBeenAdded(contexts, onEvent));
>>>>>>> master
        }

        [AdminCommand]
        [DiscordCommand("update response")]
        public async Task UpdateResponse(DiscordRequest request, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var onEvent = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "onevent")?.Value;
            var message = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "message")?.Value;
            if (onEvent == null || message == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments());
                return;
            }
            var response = await this._responsesService.GetResponseByOnEvent(onEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, onEvent));
                return;
            }
            await this._responsesService.UpdateResponse(response.Id, message);
<<<<<<< HEAD
            await messageService.SendResponse(x => x.ResponseHasBeenUpdated(contexts, onEvent, response.Message, message), contexts);
=======
            await messageService.SendResponse(x => x.ResponseHasBeenUpdated(contexts, onEvent, response.Message, message));
>>>>>>> master
        }

        [AdminCommand]
        [DiscordCommand("remove response")]
        public async Task RemoveResponse(DiscordRequest request, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var onEvent = request.Arguments.FirstOrDefault(x => x.Name?.ToLowerInvariant() == "onevent")?.Value;
            if (onEvent == null)
            {
                await messageService.SendResponse(x => x.NotEnoughArguments());
                return;
            }
            var response = await this._responsesService.GetResponseByOnEvent(onEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, onEvent));
                return;
            }
            await this._responsesService.RemoveResponse(onEvent, contexts.Server.Id);
<<<<<<< HEAD
            await messageService.SendResponse(x => x.ResponseHasBeenRemoved(contexts, onEvent), contexts);
=======
            await messageService.SendResponse(x => x.ResponseHasBeenRemoved(contexts, onEvent));
>>>>>>> master
        }

        [AdminCommand]
        [DiscordCommand("responses")]
        public async Task Responses(DiscordRequest request, Contexts contexts)
        {
            var argument = request.Arguments?.FirstOrDefault()?.Name?.ToLowerInvariant();
<<<<<<< HEAD
            if (!this.possibleArguments.Contains(argument))
=======
            if (!this._possibleArguments.Contains(argument))
>>>>>>> master
            {
                argument = "all";
            }
            await this._responsesMessageService.PrintResponses(argument, contexts);
        }
    }
}
