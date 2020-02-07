using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Devscord.DiscordFramework.Services;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;
using System.Security.Cryptography.X509Certificates;

namespace Watchman.Discord.Areas.Initialization.Controllers
{
    public class InitializationController : IController
    {
        private const string PATH_TO_RESPONSES_FILE = @"Framework/Commands/Responses/responses-configuration.json";

        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MutedRoleInitService _mutedRoleInitService;
        private readonly UsersRolesService _usersRolesService;

        public InitializationController(IQueryBus queryBus, ICommandBus commandBus, MutedRoleInitService mutedRoleInitService, UsersRolesService usersRolesService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            _mutedRoleInitService = mutedRoleInitService;
            _usersRolesService = usersRolesService;
        }

        [AdminCommand]
        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO
        public void Init(DiscordRequest request, Contexts contexts)
        {
            ResponsesInit();
            MutedRoleInit(contexts);
            UnmuteUsersInit(contexts);
        }

        private void ResponsesInit()
        {
            var responsesInBase = GetResponsesFromBase();
            var defaultResponses = GetResponsesFromFile();

            var responsesToAdd = defaultResponses.Where(d => responsesInBase.All(b => b.Id != d.Id));
            
            var command = new AddResponsesCommand(responsesToAdd);
            _commandBus.ExecuteAsync(command);
        }

        private IEnumerable<DomainModel.Responses.Response> GetResponsesFromBase()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = _queryBus.Execute(query).Responses;
            return responsesInBase;
        }

        private IEnumerable<DomainModel.Responses.Response> GetResponsesFromFile()
        {
            var fileContent = File.ReadAllText(PATH_TO_RESPONSES_FILE);
            var defaultResponses = JsonConvert.DeserializeObject<IEnumerable<DomainModel.Responses.Response>>(fileContent);
            return defaultResponses;
        }

        private void MutedRoleInit(Contexts contexts)
        {
            var mutedRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, contexts.Server);

            if (mutedRole == null)
            {
                _mutedRoleInitService.InitForServer(contexts);
            }
        }

        private void UnmuteUsersInit(Contexts contexts)
        {

        }
    }
}
