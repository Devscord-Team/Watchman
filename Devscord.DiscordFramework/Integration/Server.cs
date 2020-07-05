using System;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Services.Models;
using Discord.Rest;
using Serilog;

namespace Devscord.DiscordFramework.Integration
{
    internal static class Server
    {
        private static Services.Interfaces.IDiscordClient _discordClient;

        internal static Func<SocketGuildUser, Task> UserJoined { get; set; }
        internal static Func<SocketGuild, Task> BotAddedToServer { get; set; }
        internal static List<DateTime> ConnectedTimes => _discordClient.ServersService.ConnectedTimes;
        internal static List<DateTime> DisconnectedTimes => _discordClient.ServersService.DisconnectedTimes;

        internal static void Initialize(Services.Interfaces.IDiscordClient discordClient)
        {
            _discordClient = discordClient;
            _discordClient.UsersService.UserJoined += UserJoined;
            _discordClient.ServersService.BotAddedToServer += BotAddedToServer;
            Log.Information("Server initialized");
        }

        //Users
        internal static Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)
            => _discordClient.UsersService.GetGuildUser(userId, guildId);

        internal static Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId)
            => _discordClient.UsersService.GetGuildUsers(guildId);

        internal static Task<RestUser> GetUser(ulong userId)
            => _discordClient.UsersService.GetUser(userId);

        //Channels
        internal static Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null)
            => _discordClient.ChannelsService.GetChannel(channelId, guild);

        internal static Task SendDirectEmbedMessage(ulong userId, Embed embed)
            => _discordClient.ChannelsService.SendDirectEmbedMessage(userId, embed);

        internal static Task SendDirectMessage(ulong userId, string message)
            => _discordClient.ChannelsService.SendDirectMessage(userId, message);

        internal static Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
            => _discordClient.ChannelsService.GetMessages(server, channel, limit, fromMessageId, goBefore);

        internal static Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
            => _discordClient.RolesService.CreateNewRole(role, discordServer);

        //Roles
        internal static IEnumerable<UserRole> GetRoles(ulong guildId)
            => _discordClient.RolesService.GetRoles(guildId);

        internal static IEnumerable<SocketRole> GetSocketRoles(ulong guildId) => _discordClient.RolesService.GetSocketRoles(guildId);

        internal static Task SetRolePermissions(ChannelContext channel, ChangedPermissions permissions, UserRole role)
            => _discordClient.RolesService.SetRolePermissions(channel, permissions, role);

        internal static Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
            => _discordClient.RolesService.SetRolePermissions(channels, server, permissions, role);

        //Server
        internal static Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
            => _discordClient.ServersService.GetDiscordServers();

        internal static Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
            => _discordClient.ServersService.GetExistingInviteLinks(serverId);

        internal static Task<RestGuild> GetGuild(ulong guildId)
            => _discordClient.ServersService.GetGuild(guildId);

        internal static Task<RestGuild> GetGuild(ISocketMessageChannel data)
            => _discordClient.ServersService.GetGuild(data);
    }
}