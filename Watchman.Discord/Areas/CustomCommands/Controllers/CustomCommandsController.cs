using Devscord.DiscordFramework.Architecture.Controllers;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.CustomCommands.BotCommands;
using Watchman.Discord.Areas.Users.BotCommands;
using Watchman.DomainModel.CustomCommands.Queries;

namespace Watchman.Discord.Areas.CustomCommands.Controllers
{
    public class CustomCommandsController : IController
    {
        //todo delete custom commands logic
        private readonly IQueryBus _queryBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly IResponsesService _responsesService;

        public CustomCommandsController(IQueryBus queryBus, MessagesServiceFactory messagesServiceFactory, IResponsesService responsesService)
        {
            this._queryBus = queryBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._responsesService = responsesService;
        }
        /*
        public async Task PrintCustomCommands(CustomCommandsCommand command, Contexts contexts)
        {
            var getCustomCommandsQuery = new GetCustomCommandsQuery(contexts.Server.Id); 
            var customCommands = await this._queryBus.ExecuteAsync(getCustomCommandsQuery);
            var messagesServices = this._messagesServiceFactory.Create(contexts);
            await messagesServices.SendEmbedMessage(this._responsesService.CustomCommandsHeader(), string.Empty, customCommands.CustomCommands.Select(x => new KeyValuePair<string, string>(x.CommandFullName, x.CustomTemplateRegex)));
        }
        */
    }
}