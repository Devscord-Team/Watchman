using System;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.WebSocket;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord.Rest;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientRolesService : IDiscordClientRolesService
    {
        public Func<SocketRole, SocketRole, Task> RoleUpdated { get; set; }
        public Func<SocketRole, Task> RoleCreated { get; set; }
        public Func<SocketRole, Task> RoleRemoved { get; set; }

        public async Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            throw new NotImplementedException();
        }

        public UserRole GetRole(ulong roleId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
