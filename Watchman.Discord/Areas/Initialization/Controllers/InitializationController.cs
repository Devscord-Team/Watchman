using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Newtonsoft.Json;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Initialization.Controllers
{
    public class InitializationController : IController
    {
        private const string PATH_TO_RESPONSES_FILE = @"Framework/Commands/Responses/responses-configuration.json";

        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MuteRoleInitService _muteRoleInitService;
        private readonly UsersRolesService _usersRolesService;

        public InitializationController(IQueryBus queryBus, ICommandBus commandBus, MuteRoleInitService muteRoleInitService, UsersRolesService usersRolesService, UsersService usersService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._muteRoleInitService = muteRoleInitService;
            this._usersRolesService = usersRolesService;
        }

        [AdminCommand]
        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO //TODO co to za TODO?
        public void Init(DiscordRequest request, Contexts contexts)
        {
            _ = ResponsesInit();
            _ = MuteRoleInit(contexts);
        }

        private async Task ResponsesInit()
        {
            var responsesInBase = GetResponsesFromBase();
            var defaultResponses = GetResponsesFromFile();

            var responsesToAdd = defaultResponses.Where(def => responsesInBase.All(@base => @base.OnEvent != def.OnEvent));

            var command = new AddResponsesCommand(responsesToAdd);
            await _commandBus.ExecuteAsync(command);
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

        private async Task MuteRoleInit(Contexts contexts)
        {
            var mutedRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, contexts.Server);

            if (mutedRole == null)
            {
                await _muteRoleInitService.InitForServer(contexts);
            }
        }
    }
}
