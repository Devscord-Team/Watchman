using System.Collections.Generic;
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

        public void InitForServer(Contexts contexts)
        {
            var changedPermissions = CreateChangedPermissions();
            var mutedRole = CreateMuteRole(changedPermissions.AllowPermissions);

            var createdRole = SetRoleToServer(contexts, mutedRole);
            SetChannelsPermissions(contexts, createdRole, changedPermissions);
        }

        private UserRole CreateMuteRole(ICollection<Permission> permissions)
        {
            return new UserRole(UsersRolesService.MUTED_ROLE_NAME, permissions);
        }

        private UserRole SetRoleToServer(Contexts contexts, UserRole mutedRole)
        {
            return _usersRolesService.CreateNewRole(contexts, mutedRole);
        }

        private void SetChannelsPermissions(Contexts contexts, UserRole mutedRole, ChangedPermissions changedPermissions)
        {
            foreach (var channel in contexts.Server.TextChannels)
            {
                _channelsService.SetPermissions(channel, changedPermissions, mutedRole);
            }
        }

        private ChangedPermissions CreateChangedPermissions()
        {
            var noPermissions = new List<Permission>();
            var denyPermissions = new List<Permission> { Permission.SendMessages, Permission.SendTTSMessages, Permission.CreateInstantInvite };
            return new ChangedPermissions(noPermissions, denyPermissions);
        }
    }
}
