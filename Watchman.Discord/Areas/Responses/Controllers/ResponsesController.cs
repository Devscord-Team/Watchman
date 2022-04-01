﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.Discord.Areas.Responses.BotCommands;
using DomainResponse = Watchman.DomainModel.Responses.Response;
using Watchman.Discord.Areas.Commons;
using Watchman.Discord.ResponsesManagers;

namespace Watchman.Discord.Areas.Responses.Controllers
{
    public class ResponsesController : IController
    {
        private readonly IMessagesServiceFactory _messagesServiceFactory;
        private readonly ICustomResponsesService _responsesService;
        private readonly IResponsesMessageService _responsesMessageService;
        private readonly IResponsesCachingService responsesCachingService;

        public ResponsesController(IMessagesServiceFactory messagesServiceFactory, ICustomResponsesService responsesService, IResponsesMessageService responsesMessageService,
            IResponsesCachingService responsesCachingService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._responsesService = responsesService;
            this._responsesMessageService = responsesMessageService;
            this.responsesCachingService = responsesCachingService;
        }

        [AdminCommand]
        public async Task AddResponse(AddResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var defaultResponse = await this._responsesService.GetResponseByOnEvent(command.OnEvent, DomainResponse.DEFAULT_SERVER_ID);
            if (defaultResponse == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, command.OnEvent));
                return;
            }
            var responseForThisServer = await this._responsesService.GetResponseByOnEvent(command.OnEvent, contexts.Server.Id);
            if (responseForThisServer != null)
            {
                await messageService.SendResponse(x => x.ResponseAlreadyExists(contexts, command.OnEvent));
                return;
            }
            if (defaultResponse.Message == command.Message)
            {
                await messageService.SendResponse(x => x.ResponseAlreadyExists(contexts, command.OnEvent));
                return;
            }

            await this._responsesService.AddCustomResponse(command.OnEvent, command.Message, contexts.Server.Id);
            await messageService.SendResponse(x => x.ResponseHasBeenAdded(contexts, command.OnEvent));
            this.responsesCachingService.UpdateServerResponses(contexts.Server.Id);
        }

        [AdminCommand]
        public async Task UpdateResponse(UpdateResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var response = await this._responsesService.GetResponseByOnEvent(command.OnEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, command.OnEvent));
                return;
            }

            var defaultResponse = await this._responsesService.GetResponseByOnEvent(command.OnEvent, DomainResponse.DEFAULT_SERVER_ID);
            if (defaultResponse.Message == command.Message)
            {
                await this._responsesService.RemoveResponse(response.OnEvent, response.ServerId);
                await messageService.SendResponse(x => x.ResponseTheSameAsDefault(contexts, command.OnEvent));
                return;
            }

            await this._responsesService.UpdateResponse(response.Id, command.Message);
            await messageService.SendResponse(x => x.ResponseHasBeenUpdated(contexts, command.OnEvent, response.Message, command.Message));
            this.responsesCachingService.UpdateServerResponses(contexts.Server.Id);
        }

        [AdminCommand]
        public async Task RemoveResponse(RemoveResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var response = await this._responsesService.GetResponseByOnEvent(command.OnEvent, contexts.Server.Id);
            if (response == null)
            {
                await messageService.SendResponse(x => x.ResponseNotFound(contexts, command.OnEvent));
                return;
            }
            await this._responsesService.RemoveResponse(command.OnEvent, contexts.Server.Id);
            await messageService.SendResponse(x => x.ResponseHasBeenRemoved(contexts, command.OnEvent));
            this.responsesCachingService.UpdateServerResponses(contexts.Server.Id);
        }

        [AdminCommand]
        public Task Responses(ResponsesCommand command, Contexts contexts)
        {
            return this._responsesMessageService.PrintResponses(command, contexts);
        }
    }
}
