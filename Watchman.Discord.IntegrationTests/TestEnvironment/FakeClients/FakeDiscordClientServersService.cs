using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Discord.IntegrationTests.TestEnvironment.Models;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientServersService : IDiscordClientServersService
    {
        public Func<IGuild, Task> BotAddedToServer { get; set; } = x => Task.CompletedTask;
        public List<DateTime> DisconnectedTimes { get; set; } = new List<DateTime>();
        public List<DateTime> ConnectedTimes { get; set; } = new List<DateTime>();

        private List<ChannelContext> channelContexts = new List<ChannelContext>()
        {
            new ChannelContext(0, "zero"),
            new ChannelContext(1, "a"),
            new ChannelContext(2, "b"),
            new ChannelContext(3, "c"),
            new ChannelContext(4, "d"),
            new ChannelContext(5, "e"),
        };

        public Task<IGuild> GetGuild(ulong guildId)
        {
            var guild = new FakeGuild()
            {
                Id = 1,
                Name = "TestServer",
                OwnerId = 5,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                Roles = new List<IRole>()
                {
                    new FakeRole() { Id = 1, Name = "First", CreatedAt = DateTime.UtcNow.AddDays(-5), },
                    new FakeRole() { Id = 2, Name = "Second", CreatedAt = DateTime.UtcNow.AddDays(-5), },
                    new FakeRole() { Id = 3, Name = "Third", CreatedAt = DateTime.UtcNow.AddDays(-5), },
                }
            } as IGuild;
            return Task.FromResult(guild);
        }

        public async IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync()
        {
            var item = await this.GetDiscordServerAsync(1);
            yield return item;
        }

        public Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId)
        {
            var landingChannel = new ChannelContext(1, "landing");
            var server = new DiscordServerContext(1, "Test", () => new UserContext(1, "test", null, null, null, null, null), landingChannel, _ => this.channelContexts, _ => null, _ => null);
            return Task.FromResult(server);
        }

        public Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            throw new NotImplementedException();
        }
    }
}
