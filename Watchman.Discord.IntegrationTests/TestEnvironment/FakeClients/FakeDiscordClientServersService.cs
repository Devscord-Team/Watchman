﻿using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientServersService : IDiscordClientServersService
    {
        public Func<SocketGuild, Task> BotAddedToServer { get; set; } = x => Task.CompletedTask;
        public List<DateTime> DisconnectedTimes { get; set; } = new List<DateTime>();
        public List<DateTime> ConnectedTimes { get; set; } = new List<DateTime>();

        public async Task<RestGuild> GetGuild(ulong guildId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            throw new NotImplementedException();
        }
    }
}
