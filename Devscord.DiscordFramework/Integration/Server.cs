using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Models;
using Discord;
using Discord.Rest;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

namespace Devscord.DiscordFramework.Integration
{
    internal static class Server
    {
        private static Services.Interfaces.IDiscordClient _discordClient;

        internal static Func<SocketGuildUser, Task> UserJoined { get; set; }
        internal static Func<SocketGuild, Task> BotAddedToServer { get; set; }
        internal static Func<SocketChannel, Task> ChannelCreated { get; set; }
        internal static Func<SocketRole, SocketRole, Task> RoleUpdated { get; set; }
        internal static Func<SocketRole, Task> RoleRemoved { get; set; }
        internal static Func<SocketRole, Task> RoleCreated { get; set; }
        internal static List<DateTime> ConnectedTimes => _discordClient.ServersService.ConnectedTimes;
        internal static List<DateTime> DisconnectedTimes => _discordClient.ServersService.DisconnectedTimes;

        internal static void Initialize(Services.Interfaces.IDiscordClient discordClient)
        {
            _discordClient = discordClient;
            _discordClient.UsersService.UserJoined += UserJoined;
            _discordClient.ServersService.BotAddedToServer += BotAddedToServer;
            _discordClient.ChannelsService.ChannelCreated += ChannelCreated;
            _discordClient.RolesService.RoleUpdated += RoleUpdated;
            _discordClient.RolesService.RoleCreated += RoleCreated;
            _discordClient.RolesService.RoleRemoved += RoleRemoved;
            Log.Information("Server initialized");
        }

        //Users
        internal static Task<RestGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            return _discordClient.UsersService.GetGuildUser(userId, guildId);
        }

        internal static Task<IEnumerable<RestGuildUser>> GetGuildUsers(ulong guildId)
        {
            return _discordClient.UsersService.GetGuildUsers(guildId);
        }

        internal static Task<RestUser> GetUser(ulong userId)
        {
            return _discordClient.UsersService.GetUser(userId);
        }

        //Channels
        internal static Task<IChannel> GetChannel(ulong channelId, RestGuild guild = null)
        {
            return _discordClient.ChannelsService.GetChannel(channelId, guild);
        }

        internal static Task<IGuildChannel> GetGuildChannel(ulong channelId, RestGuild guild = null)
        {
            return _discordClient.ChannelsService.GetGuildChannel(channelId, guild);
        }

        internal static Task SendDirectEmbedMessage(ulong userId, Embed embed)
        {
            return _discordClient.ChannelsService.SendDirectEmbedMessage(userId, embed);
        }

        internal static Task SendDirectMessage(ulong userId, string message)
        {
            return _discordClient.ChannelsService.SendDirectMessage(userId, message);
        }

        internal static Task<IEnumerable<Message>> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            return _discordClient.ChannelsService.GetMessages(server, channel, limit, fromMessageId, goBefore);
        }

        internal static Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            return _discordClient.RolesService.CreateNewRole(role, discordServer);
        }

        //Roles
        internal static IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            return _discordClient.RolesService.GetRoles(guildId);
        }

        internal static IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            return _discordClient.RolesService.GetSocketRoles(guildId);
        }

        internal static Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            return _discordClient.RolesService.SetRolePermissions(channel, server, permissions, role);
        }

        internal static Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            return _discordClient.RolesService.SetRolePermissions(channels, server, permissions, role);
        }

        //Server
        internal static Task<IEnumerable<DiscordServerContext>> GetDiscordServers()
        {
            return _discordClient.ServersService.GetDiscordServers();
        }

        internal static Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            return _discordClient.ServersService.GetExistingInviteLinks(serverId);
        }

        internal static Task<RestGuild> GetGuild(ulong guildId)
        {
            return _discordClient.ServersService.GetGuild(guildId);
        }
    }
}