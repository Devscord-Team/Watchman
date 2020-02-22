using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class MuteRoleInitService
    {
        private readonly UsersRolesService _usersRolesService;
        private readonly ChannelsService _channelsService;

        public MuteRoleInitService(UsersRolesService usersRolesService, ChannelsService channelsService)
        {
            _usersRolesService = usersRolesService;
            _channelsService = channelsService;
        }

        public Task InitForServer(Contexts contexts)
        {
            var changedPermissions = CreateChangedPermissions();
            var mutedRole = CreateMuteRole(changedPermissions.AllowPermissions);

            var createdRole = SetRoleToServer(contexts, mutedRole);
            SetChannelsPermissions(contexts, createdRole, changedPermissions);
            return Task.CompletedTask;
        }

        private NewUserRole CreateMuteRole(ICollection<Permission> permissions)
        {
            return new NewUserRole(UsersRolesService.MUTED_ROLE_NAME, permissions);
        }

        private UserRole SetRoleToServer(Contexts contexts, NewUserRole mutedRole)
        {
            return _usersRolesService.CreateNewRole(contexts, mutedRole);
        }

        private Task SetChannelsPermissions(Contexts contexts, UserRole mutedRole, ChangedPermissions changedPermissions)
        {
            Parallel.ForEach(contexts.Server.TextChannels,
                channel => _channelsService.SetPermissions(channel, changedPermissions, mutedRole));
            return Task.CompletedTask;
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var noPermissions = new List<Permission>();
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite };
            return new ChangedPermissions(noPermissions, denyPermissions);
        }
    }
}
