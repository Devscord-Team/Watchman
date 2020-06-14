using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Models;
using Discord.Rest;

namespace Devscord.DiscordFramework.Integration.Services.Interfaces
{
    internal interface IDiscordClientChannelsService
    {
        Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null);
        Task SendDirectEmbedMessage(ulong userId, Embed embed);
        Task SendDirectMessage(ulong userId, string message);
        Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true);
        bool CanBotReadTheChannel(IMessageChannel textChannel);
    }
}
