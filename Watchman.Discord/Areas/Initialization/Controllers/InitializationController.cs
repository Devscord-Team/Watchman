using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
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
        private readonly UsersRolesService _usersRolesService;
        private readonly ChannelsService _channelsService;

        public InitializationController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory,
                                        UsersRolesService usersRolesService, ChannelsService channelsService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._usersRolesService = usersRolesService;
            this._channelsService = channelsService;
        }

        [AdminCommand]
        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO
        public void Init(DiscordRequest request, Contexts contexts)
        {
            ResponsesInit();
            var changedPermissions = CreateChangedPermissions();
            var mutedRole = CreateMuteRole(changedPermissions.AllowPermissions);

            var rolesBeforeCount = _usersRolesService.GetRoles(contexts.Server).Count();

            var createdRole = SetRoleToServer(contexts, mutedRole);

            SetChannelsPermissions(contexts, createdRole, changedPermissions);
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

        private UserRole CreateMuteRole(Permissions permissions)
        {
            return new UserRole("muted", permissions.ToList());
        }

        private UserRole SetRoleToServer(Contexts contexts, UserRole mutedRole)
        {
            return _usersRolesService.CreateNewRole(contexts, mutedRole);
        }

        private void SetChannelsPermissions(Contexts contexts, UserRole mutedRole, ChangedPermissions changedPermissions)
        {
            foreach (var channel in contexts.Server.TextChannels)
            {
                _channelsService.SetPermissions(contexts.Server, channel, changedPermissions, mutedRole);
            }
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var onlyReadPermission = new List<Permission> { Permission.ReadMessages };
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite };
            return new ChangedPermissions(onlyReadPermission, denyPermissions);
        }
    }
}
