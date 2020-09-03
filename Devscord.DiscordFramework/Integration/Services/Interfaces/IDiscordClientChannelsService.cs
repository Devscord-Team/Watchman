using System;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientChannelsService
    {
        Func<SocketChannel, Task> ChannelCreated { get; set; }
        Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null);
        Task<IGuildChannel> GetGuildChannel(ulong channelId, RestGuild guild);
        Task SendDirectEmbedMessage(ulong userId, Embed embed);
        Task SendDirectMessage(ulong userId, string message);
        IAsyncEnumerable<Message> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true);
        bool CanBotReadTheChannel(IMessageChannel textChannel);
        Task<ITextChannel> CreateNewChannelAsync(ulong serverId, string channelName);
    }
}
