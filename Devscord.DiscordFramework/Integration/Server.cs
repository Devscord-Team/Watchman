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
using System.IO;

namespace Devscord.DiscordFramework.Integration
{
    internal static class Server
    {
        private static Services.Interfaces.IDiscordClient _discordClient;

        internal static Func<SocketGuildUser, Task> UserJoined { get; set; }
        internal static Func<SocketGuild, Task> BotAddedToServer { get; set; }
        internal static Func<SocketChannel, Task> ChannelCreated { get; set; }
        internal static Func<SocketChannel, Task> ChannelRemoved { get; set; }
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
            _discordClient.ChannelsService.ChannelRemoved += ChannelRemoved;
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

        internal static IEnumerable<IGuildUser> GetGuildUsers(ulong guildId)
        {
            return _discordClient.UsersService.GetGuildUsers(guildId);
        }

        internal static Task<RestUser> GetUser(ulong userId)
        {
            return _discordClient.UsersService.GetUser(userId);
        }

        internal static Task<bool> IsUserStillOnServer(ulong userId, ulong guildId)
        {
            return _discordClient.UsersService.IsUserStillOnServer(userId, guildId);
        }

        internal static RestUser GetBotUser()
        {
            return _discordClient.UsersService.GetBotUser();
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

        internal static Task SendDirectFile(ulong userId, string fileName, Stream stream)
        {
            return _discordClient.ChannelsService.SendDirectFile(userId, fileName, stream);
        }

        internal static IAsyncEnumerable<Message> GetMessages(DiscordServerContext server, ChannelContext channel, int limit, ulong fromMessageId = 0, bool goBefore = true)
        {
            return _discordClient.ChannelsService.GetMessages(server, channel, limit, fromMessageId, goBefore);
        }

        internal static Task<ITextChannel> CreateNewChannel(ulong serverId, string channelName)
        {
            return _discordClient.ChannelsService.CreateNewChannelAsync(serverId, channelName);
        }

        internal static Task SetRolePermissions(ChannelContext channel, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            return _discordClient.ChannelsService.SetRolePermissions(channel, server, permissions, role);
        }

        internal static Task SetRolePermissions(IEnumerable<ChannelContext> channels, DiscordServerContext server, ChangedPermissions permissions, UserRole role)
        {
            return _discordClient.ChannelsService.SetRolePermissions(channels, server, permissions, role);
        }

        internal static Task RemoveRolePermissions(ChannelContext channel, DiscordServerContext server, UserRole role)
        {
            return _discordClient.ChannelsService.RemoveRolePermissions(channel, server, role);
        }

        //Roles
        internal static Task<UserRole> CreateNewRole(NewUserRole role, DiscordServerContext discordServer)
        {
            return _discordClient.RolesService.CreateNewRole(role, discordServer);
        }

        internal static IEnumerable<UserRole> GetRoles(ulong guildId)
        {
            return _discordClient.RolesService.GetRoles(guildId);
        }

        internal static IEnumerable<SocketRole> GetSocketRoles(ulong guildId)
        {
            return _discordClient.RolesService.GetSocketRoles(guildId);
        }

        internal static UserRole GetRole(ulong roleId, ulong guildId)
        {
            return _discordClient.RolesService.GetRole(roleId, guildId);
        }

        //Server
        internal static IAsyncEnumerable<DiscordServerContext> GetDiscordServersAsync()
        {
            return _discordClient.ServersService.GetDiscordServersAsync();
        }

        internal static Task<DiscordServerContext> GetDiscordServerAsync(ulong serverId)
        {
            return _discordClient.ServersService.GetDiscordServerAsync(serverId);
        }

        internal static Task<IEnumerable<string>> GetExistingInviteLinks(ulong serverId)
        {
            return _discordClient.ServersService.GetExistingInviteLinks(serverId);
        }

        internal static Task<RestGuild> GetGuild(ulong guildId)
        {
            return _discordClient.ServersService.GetGuild(guildId);
        }

        internal static Task<ulong[]> GetUsersIdsFromServer(ulong serverId)
        {
            return _discordClient.ServersService.GetUsersIdsFromServer(serverId);
        }
    }
}