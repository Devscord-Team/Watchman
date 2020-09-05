using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Watchman.Discord.Areas.Protection.BotCommands;

namespace Watchman.Discord.Areas.Protection.Controllers
{
    public class AdministrationController : IController
    {
        private readonly ChannelsService _channelsService;
        private readonly DiscordServersService _discordServersService;
        private readonly UsersRolesService _usersRolesService;

        public AdministrationController(ChannelsService channelsService, DiscordServersService discordServersService, UsersRolesService usersRolesService)
        {
            this._channelsService = channelsService;
            this._discordServersService = discordServersService;
            this._usersRolesService = usersRolesService;
        }

        [AdminCommand]
        public async Task CreateChannelForComplaints(ComplaintsChannelCommand command, Contexts contexts)
        {
            var complaintsChannel = contexts.Server.GetTextChannels().FirstOrDefault(x => x.Name == "skargi")
                ?? await this._channelsService.CreateNewChannelAsync(contexts.Server, "skargi");

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

            
            // add the channel's ID to a base
            // assing action to ChannelRemoved to remove complaintsChannelId from the base, when the channel is deleted
        }
    }
}
