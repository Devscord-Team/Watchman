using System;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Responses.Services;
using Watchman.Discord.Areas.Responses.BotCommands;
using Serilog;
using DomainResponse = Watchman.DomainModel.Responses.Response;

namespace Watchman.Discord.Areas.Responses.Controllers
{
    public class ResponsesController : IController
    {
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly Services.ResponsesService _responsesService;
        private readonly ResponsesMessageService _responsesMessageService;

        public ResponsesController(MessagesServiceFactory messagesServiceFactory, Services.ResponsesService responsesService, ResponsesMessageService responsesMessageService)
        {
            this._messagesServiceFactory = messagesServiceFactory;
            this._responsesService = responsesService;
            this._responsesMessageService = responsesMessageService;
        }

        [AdminCommand]
        public async Task AddResponse(AddResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var onEvent = command.OnEvents?.FirstOrDefault();
            var message = command.Messages?.FirstOrDefault();
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

            var defaultResponse = await this._responsesService.GetResponseByOnEvent(onEvent, DomainResponse.DEFAULT_SERVER_ID);
            if (defaultResponse.Message == message)
            {
                await messageService.SendResponse(x => x.ResponseAlreadyExists(contexts, onEvent));
                return;
            }

            await this._responsesService.AddResponse(onEvent, message, contexts.Server.Id);
            await messageService.SendResponse(x => x.ResponseHasBeenAdded(contexts, onEvent));
        }

        [AdminCommand]
        public async Task UpdateResponse(UpdateResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var onEvent = command.OnEvents?.FirstOrDefault();
            var message = command.Messages?.FirstOrDefault();
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

            var defaultResponse = await this._responsesService.GetResponseByOnEvent(onEvent, DomainResponse.DEFAULT_SERVER_ID);
            if (defaultResponse.Message == message)
            {
                await this._responsesService.RemoveResponse(response.OnEvent, response.ServerId);
                await messageService.SendResponse(x => x.ResponseTheSameAsDefault(contexts, onEvent));
                return;
            }

            await this._responsesService.UpdateResponse(response.Id, message);
            await messageService.SendResponse(x => x.ResponseHasBeenUpdated(contexts, onEvent, response.Message, message));
        }

        [AdminCommand]
        public async Task RemoveResponse(RemoveResponseCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);
            var onEvent = command.OnEvents?.FirstOrDefault();
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
            await messageService.SendResponse(x => x.ResponseHasBeenRemoved(contexts, onEvent));
        }

        [AdminCommand]
        public async Task Responses(ResponsesCommand responseCommand, Contexts contexts)
        {
            string argument;
            if (responseCommand.Default)
            {
                argument = "default";
            }
            else if (responseCommand.Custom)
            {
                argument = "custom";
            }
            else
            {
                argument = "all";
            }
            await this._responsesMessageService.PrintResponses(argument, contexts);
        }
    }
}
