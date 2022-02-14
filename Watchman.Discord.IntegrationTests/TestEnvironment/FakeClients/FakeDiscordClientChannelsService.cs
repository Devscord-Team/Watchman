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

namespace Watchman.Discord.IntegrationTests.TestEnvironment.FakeClients
{
    internal class FakeDiscordClientChannelsService : IDiscordClientChannelsService
    {
        public Func<SocketChannel, Task> ChannelCreated { get; set; }
        public Func<SocketChannel, Task> ChannelRemoved { get; set; }

        public async Task SendDirectMessage(ulong userId, string message)
        {
            throw new NotImplementedException();
        }

        public async Task SendDirectEmbedMessage(ulong userId, Embed embed)
        {
            throw new NotImplementedException();
        }

        public async Task SendDirectFile(ulong userId, string fileName, Stream stream)
        {
            throw new NotImplementedException();
        }

        public async Task<IChannel> GetChannel(ulong channelId, IGuild guild)
        {
            throw new NotImplementedException();
        }

        public IChannel GetChannel(ulong channelId, ulong serverId)
        {
            throw new NotImplementedException();
        }

        public async Task<IGuildChannel> GetGuildChannel(ulong channelId, RestGuild guild = null)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<Message> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CanBotReadTheChannelAsync(IMessageChannel textChannel)
        {
            throw new NotImplementedException();
        }

        public async Task<ITextChannel> CreateNewChannelAsync(ulong serverId, string channelName)
        {
            throw new NotImplementedException();
        }

        public async Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            throw new NotImplementedException();
        }

        public Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveRolePermissions(ChannelContext channel, DiscordServerContext server, UserRole role)
        {
            throw new NotImplementedException();
        }
    }
}
