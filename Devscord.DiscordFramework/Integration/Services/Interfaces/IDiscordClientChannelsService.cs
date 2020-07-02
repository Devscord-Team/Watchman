using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Discord;
using Discord.Rest;
using System.Collections.Generic;
using System.Threading.Tasks;

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
