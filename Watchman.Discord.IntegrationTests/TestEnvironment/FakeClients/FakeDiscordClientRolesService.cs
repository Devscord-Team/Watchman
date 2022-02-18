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
        public Func<IRole, IRole, Task> RoleUpdated { get; set; }
        public Func<IRole, Task> RoleCreated { get; set; }
        public Func<IRole, Task> RoleRemoved { get; set; }

        public Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            throw new NotImplementedException();
        }

        public UserRole GetRole(ulong roleId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IRole> GetSocketRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }
    }
}
