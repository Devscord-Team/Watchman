using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Initialization.Controllers
{
    public class InitializationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public InitializationController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO
        public void Init(DiscordRequest request, Contexts contexts)
        {
            ResponsesInit();
            CreateMuteRole();
            SetChannelsPermissions();
        }

        private void ResponsesInit()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = _queryBus.Execute(query).Responses;

            if (!responsesInBase.Any())
            {
                var fileContent = File.ReadAllText(@"Framework/Commands/Responses/responses-configuration.json");
                var responsesToAdd = JsonConvert.DeserializeObject<IEnumerable<DomainModel.Responses.Response>>(fileContent);
                var command = new AddResponsesCommand(responsesToAdd);
                _commandBus.ExecuteAsync(command);
            }
        }

        private void CreateMuteRole()
        {
            
        }

        private void SetChannelsPermissions()
        {

        }
    }
}
