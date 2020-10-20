using System;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
<<<<<<< HEAD
using System.IO;
=======
using Devscord.DiscordFramework.Commons;
>>>>>>> master

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientChannelsService
    {
        Func<SocketChannel, Task> ChannelCreated { get; set; }
        Func<SocketChannel, Task> ChannelRemoved { get; set; }
        Task<IChannel> GetChannel(ulong channelId, IGuild guild);
        IChannel GetChannel(ulong channelId, ulong serverId);
        Task<IGuildChannel> GetGuildChannel(ulong channelId, RestGuild guild);
        Task SendDirectEmbedMessage(ulong userId, Embed embed);
        Task SendDirectMessage(ulong userId, string message);
        IAsyncEnumerable<Message> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true);
<<<<<<< HEAD
        Task<bool> CanBotReadTheChannel(IMessageChannel textChannel);
        Task SendDirectFile(ulong userId, string fileName, Stream stream);
=======
        Task<bool> CanBotReadTheChannelAsync(IMessageChannel textChannel);
        Task<ITextChannel> CreateNewChannelAsync(ulong serverId, string channelName);
        Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole role);
        Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role);
        Task RemoveRolePermissions(ChannelContext channel, DiscordServerContext server, UserRole role);
>>>>>>> master
    }
}
