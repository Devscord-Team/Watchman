using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Protection.BotCommands;
using Watchman.DomainModel.Protection.Commands;
using Watchman.DomainModel.Protection.Queries;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly ChannelsService _channelsService;
        private readonly UsersRolesService _usersRolesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public AdministrationController(IQueryBus queryBus, ICommandBus commandBus, ChannelsService channelsService, UsersRolesService usersRolesService, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._channelsService = channelsService;
            this._usersRolesService = usersRolesService;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        public async Task CreateChannelForComplaints(ComplaintsChannelCommand command, Contexts contexts)
        {
            // create channel
            var complaintsChannelName = string.IsNullOrWhiteSpace(command.Name) ? "skargi" : command.Name;

            var query = new GetComplaintsChannelQuery(contexts.Server.Id);
            var complaintsChannelId = this._queryBus.Execute(query).ComplaintsChannel.ChannelId;

            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (complaintsChannelId == 0)
            {
                await messagesService.SendResponse(x => x.ComplaintsChannelAlreadyExists());
                return;
            }

            var complaintsChannel = contexts.Server.GetTextChannels().FirstOrDefault(x => x.Id == complaintsChannelId)
                ?? await this._channelsService.CreateNewChannelAsync(contexts.Server, complaintsChannelName);
            // end create channel

            //set perms
            var readingAndSending = new List<Permission> { Permission.ReadMessages, Permission.SendMessages };

            var everyonePermissions = new ChangedPermissions(allowPermissions: null, denyPermissions: readingAndSending);
            var adminsPermissions = new ChangedPermissions(readingAndSending, null);
            var mutedPermissions = new ChangedPermissions(readingAndSending, new List<Permission> { Permission.ReadMessageHistory });

            var serverRoles = this._usersRolesService.GetRoles(contexts.Server).ToList();
            var everyoneRole = serverRoles.First(x => x.Name == "@everyone");
            var adminRoles = serverRoles.Where(x => x.Permissions.Contains(Permission.ManageGuild));
            var mutedRole = serverRoles.FirstOrDefault(x => x.Name == UsersRolesService.MUTED_ROLE_NAME);

            var setAdminPerms = adminRoles.Select(role => this._channelsService.SetPermissions(complaintsChannel, contexts.Server, adminsPermissions, role));
            Task.WaitAll(setAdminPerms.ToArray());
            await this._channelsService.SetPermissions(complaintsChannel, contexts.Server, mutedPermissions, mutedRole);
            await this._channelsService.SetPermissions(complaintsChannel, contexts.Server, everyonePermissions, everyoneRole);
            // end set perms

            var addComplaintsChannelCommand = new AddComplaintsChannelCommand(complaintsChannel.Id);
            await this._commandBus.ExecuteAsync(addComplaintsChannelCommand);

            await messagesService.SendResponse(x => x.ComplaintsChannelHasBeenCreated());

            // assign action to ChannelRemoved to remove complaintsChannelId from the base, when the channel is deleted
        }
    }
}
