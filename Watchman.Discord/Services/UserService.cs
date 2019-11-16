using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using Watchman.Discord.Framework;
using Watchman.Discord.Framework.Architecture.Middlewares;
using Watchman.Discord.Middlewares.Contexts;

namespace Watchman.Discord.Services
{
    class UserService
    {
        public Task AddRole(ulong roleId, Dictionary<string, IDiscordContext> contexts)
        {
            var user = GetUser(contexts);
            var role = GetRole(roleId, user);
            return user.AddRoleAsync(role);
        }

        public Task RemoveRole(ulong roleId, Dictionary<string, IDiscordContext> contexts)
        {
            var user = GetUser(contexts);
            var role = GetRole(roleId, user);
            return user.RemoveRoleAsync(role);
        }

        private SocketGuildUser GetUser(Dictionary<string, IDiscordContext> contexts)
        {
            var userId = ((UserContext) contexts[nameof(UserContext)]).Id;
            return (SocketGuildUser)Server.GetUser(userId);
        }

        private SocketRole GetRole(ulong roleId, SocketGuildUser user)
        {
            return Server.GetRoles(user.Guild.Id).First(x => x.Id == roleId);
        }
    }
}
