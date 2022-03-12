using System;
using Devscord.DiscordFramework.Commands.Parsing;
using Devscord.DiscordFramework.Integration.Services.Interfaces;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Middlewares.Factories;
using Devscord.DiscordFramework.Services.Models;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Commands.Parsing.Models;
using System.IO;
using Watchman.Discord.IntegrationTests.TestEnvironment.Models;

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientChannelsService : IDiscordClientChannelsService
    {
        public Func<IChannel, Task> ChannelCreated { get; set; }
        public Func<IChannel, Task> ChannelRemoved { get; set; }

        public Task SendDirectMessage(ulong userId, string message)
        {
            throw new NotImplementedException();
        }

        public Task SendDirectEmbedMessage(ulong userId, Embed embed)
        {
            throw new NotImplementedException();
        }

        public Task SendDirectFile(ulong userId, string fileName, Stream stream)
        {
            throw new NotImplementedException();
        }

        public Task<IChannel> GetChannel(ulong channelId, IGuild guild)
        {
            return Task.FromResult(new FakeChannel() { Id = 5, Name = "TestSystemChannel" } as IChannel);
        }

        public IChannel GetChannel(ulong channelId, ulong serverId)
        {
            return new FakeChannel() { Id = 5, Name = "TestSystemChannel" } as IChannel;
        }

        public Task<IGuildChannel> GetGuildChannel(ulong channelId, IGuild guild = null)
        {
            return Task.FromResult(new FakeChannel() { Id = 5, Name = "TestSystemChannel" } as IGuildChannel);
        }

        public IAsyncEnumerable<Message> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanBotReadTheChannelAsync(IMessageChannel textChannel)
        {
            return Task.FromResult(false);
        }

        public Task<ITextChannel> CreateNewChannelAsync(ulong serverId, string channelName)
        {
            return Task.FromResult(new FakeChannel() { Id = 5, Name = "TestSystemChannel" } as ITextChannel);
        }

        public Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            return Task.CompletedTask;
        }

        public Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            return Task.CompletedTask;
        }

        public Task RemoveRolePermissions(ChannelContext channel, DiscordServerContext server, UserRole role)
        {
            return Task.CompletedTask;
        }
    }
}
