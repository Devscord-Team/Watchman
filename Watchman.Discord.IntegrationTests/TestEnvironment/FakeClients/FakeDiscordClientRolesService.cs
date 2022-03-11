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
            return Task.FromResult(new UserRole(99, "createdRole", 1));
        }

        public UserRole GetRole(ulong roleId, ulong guildId)
        {
            return new UserRole(roleId, "generatedRole", 1);
        }

        public IEnumerable<IRole> GetSocketRoles(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            return new List<UserRole>()
            {
                new UserRole(1, "1", 1),
                new UserRole(2, "2", 1),
                new UserRole(3, "3", 1),
                new UserRole(4, "4", 1),
            };
        }
    }
}
