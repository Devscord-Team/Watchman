using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord;
using Discord.Rest;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class UserContextsFactory : IContextFactory<IUser, UserContext>
    {
        private readonly UsersRolesService _usersRolesService;
        private readonly DiscordServersService _discordServersService;

        public UserContextsFactory(UsersRolesService usersRolesService, DiscordServersService discordServersService)
        {
            this._usersRolesService = usersRolesService;
            this._discordServersService = discordServersService;
        }

        public UserContext Create(IUser user)
        {
            var socketGuildUser = user as RestGuildUser;
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);
            if (socketGuildUser is null)
            {
                return new UserContext(user.Id, user.ToString(), new List<UserRole>(), avatarUrl, user.Mention, false);
            }
            var roles = socketGuildUser.RoleIds.Select(x => this._usersRolesService.GetRole(x, socketGuildUser.GuildId));
            var owner = this._discordServersService.GetDiscordServerAsync(socketGuildUser.GuildId).Result;
            var isOwner = owner.Id == user.Id;
            return new UserContext(user.Id, user.ToString(), roles.ToList(), avatarUrl, user.Mention, isOwner);
        }
    }
}
