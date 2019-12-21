using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Initialization.Controllers
{
    public class InitializationController : IController
    {
        private readonly IQueryBus queryBus;
        private readonly ICommandBus commandBus;
        private readonly MessagesServiceFactory messagesServiceFactory;

        public InitializationController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory)
        {
            this.queryBus = queryBus;
            this.commandBus = commandBus;
            this.messagesServiceFactory = messagesServiceFactory;
        }

        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO
        public void Init(DiscordRequest request, Contexts contexts)
        {
            ResponsesInit();
        }

        private void ResponsesInit()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = queryBus.Execute(query).Responses;

            if (!responsesInBase.Any())
            {
                var fileContent = File.ReadAllText(@"Framework/Commands/Responses/responses-configuration.json");
                var responsesToAdd = JsonConvert.DeserializeObject<IEnumerable<DomainModel.Responses.Response>>(fileContent);
                var command = new AddResponsesCommand(responsesToAdd);
                commandBus.ExecuteAsync(command);
            }
        }

    }
}
