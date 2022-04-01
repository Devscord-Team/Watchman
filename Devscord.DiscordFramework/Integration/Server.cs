﻿using Devscord.DiscordFramework.Commons;
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
    internal static class Server //todo singleton instad static class
    {
        private static Services.Interfaces.IDiscordClient _discordClient;

        internal static Func<IMessage, Task> MessageReceived { get; set; }
        internal static Func<IGuildUser, Task> UserJoined { get; set; }
        internal static Func<IGuild, Task> BotAddedToServer { get; set; }
        internal static Func<IChannel, Task> ChannelCreated { get; set; }
        internal static Func<IChannel, Task> ChannelRemoved { get; set; }
        internal static Func<IRole, IRole, Task> RoleUpdated { get; set; }
        internal static Func<IRole, Task> RoleRemoved { get; set; }
        internal static Func<IRole, Task> RoleCreated { get; set; }
        internal static List<DateTime> ConnectedTimes => _discordClient.ServersService.ConnectedTimes;
        internal static List<DateTime> DisconnectedTimes => _discordClient.ServersService.DisconnectedTimes;

        internal static void Initialize(Services.Interfaces.IDiscordClient discordClient) //todo use IDiscordClient over Server
        {
            _discordClient = discordClient;
            _discordClient.MessagesService.MessageReceived += MessageReceived;
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
        internal static Task<IGuildUser> GetGuildUser(ulong userId, ulong guildId)
        {
            return _discordClient.UsersService.GetGuildUser(userId, guildId);
        }

        internal static IEnumerable<IGuildUser> GetGuildUsers(ulong guildId)
        {
            return _discordClient.UsersService.GetGuildUsers(guildId);
        }

        internal static Task<IUser> GetUser(ulong userId)
        {
            return _discordClient.UsersService.GetUser(userId);
        }

        internal static Task<bool> IsUserStillOnServer(ulong userId, ulong guildId)
        {
            return _discordClient.UsersService.IsUserStillOnServer(userId, guildId);
        }

        internal static IUser GetBotUser()
        {
            return _discordClient.UsersService.GetBotUser();
        }

        //Channels
        internal static Task<IChannel> GetChannel(ulong channelId, IGuild guild = null)
        {
            return _discordClient.ChannelsService.GetChannel(channelId, guild);
        }

        internal static Task<IGuildChannel> GetGuildChannel(ulong channelId, IGuild guild = null)
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

        internal static IEnumerable<IRole> GetSocketRoles(ulong guildId)
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

        internal static Task<IGuild> GetGuild(ulong guildId)
        {
            return _discordClient.ServersService.GetGuild(guildId);
        }
    }
}