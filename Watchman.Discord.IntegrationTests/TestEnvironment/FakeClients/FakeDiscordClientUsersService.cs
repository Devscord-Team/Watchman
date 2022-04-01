using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Watchman.Discord.IntegrationTests.TestEnvironment.Models;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientUsersService : IDiscordClientUsersService
    {
        public Func<IGuildUser, Task> UserJoined { get; set; }

        public Task<IUser> GetUser(ulong userId)
        {
            IUser user = new FakeUser()
            {
                Id = 3u,
                IsBot = false,
                IsWebhook = false,
                Username = "TestUser",
                Mention = "<@12345678>",
                Status = UserStatus.Online,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            };
            return Task.FromResult(user);
        }

        public Task<bool> IsUserStillOnServer(ulong userId, ulong guildId)
        {
            throw new NotImplementedException();
        }

        public Task<IGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            IGuildUser user = new FakeUser()
            {
                Id = 3u,
                IsBot = false,
                IsWebhook = false,
                Username = "TestUser",
                Mention = "<@12345678>",
                Status = UserStatus.Online,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            };
            return Task.FromResult(user);
        }

        public IEnumerable<IGuildUser> GetGuildUsers(ulong guildId)
        {
            IGuildUser user = new FakeUser()
            {
                Id = 3u,
                IsBot = false,
                IsWebhook = false,
                Username = "TestUser",
                Mention = "<@12345678>",
                Status = UserStatus.Online,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            };
            return new List<IGuildUser> { user };
        }

        public IUser GetBotUser()
        {
            IUser user = new FakeUser()
            {
                Id = 8u,
                IsBot = false,
                IsWebhook = false,
                Username = "TestUser",
                Mention = "<@12345678>",
                Status = UserStatus.Online,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
            };
            return user;
        }
    }
}
