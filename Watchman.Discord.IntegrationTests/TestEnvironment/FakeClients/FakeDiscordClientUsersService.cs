using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientUsersService : IDiscordClientUsersService
    {
        public Func<IGuildUser, Task> UserJoined { get; set; }

        public Task<IUser> GetUser(ulong userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserStillOnServer(ulong userId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IGuildUser> GetGuildUsers(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public RestUser GetBotUser()
        {
            throw new NotImplementedException();
        }
    }
}
