using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.DomainModel.Protection.Commands;
using Watchman.DomainModel.Protection.Queries;

namespace Watchman.Discord.Areas.Protection.Services
{
    public class ComplaintsChannelService
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly ChannelsService _channelsService;
        private readonly UsersRolesService _usersRolesService;

        public ComplaintsChannelService(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory, ChannelsService channelsService, UsersRolesService usersRolesService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
            this._channelsService = channelsService;
            this._usersRolesService = usersRolesService;
        }

        public Task RemoveIfNeededComplaintsChannel(ChannelContext channel, DiscordServerContext server)
        {
            var query = new GetComplaintsChannelQuery(server.Id);
            var complaintsChannel = this._queryBus.Execute(query).ComplaintsChannel;
            if (channel.Id != complaintsChannel.ChannelId)
            {
                return Task.CompletedTask;
            }
            var command = new RemoveComplaintsChannelCommand(complaintsChannel);
            return this._commandBus.ExecuteAsync(command);
        }

        public async Task<ChannelContext> CreateComplaintsChannel(string channelName, Contexts contexts)
        {
            if (this.IsComplaintsChannelAlreadyExisting(contexts.Server))
            {
                throw new ComplaintsChannelAlreadyExistsException();
            }
            var complaintsChannel = await this._channelsService.CreateNewChannelAsync(contexts.Server, channelName);
            await this.SetChannelPermissions(complaintsChannel, contexts);
            await this.NotifyDomainAboutComplaintsChannel(complaintsChannel, contexts.Server);
            return complaintsChannel;
        }

        private bool IsComplaintsChannelAlreadyExisting(DiscordServerContext server)
        {
            var query = new GetComplaintsChannelQuery(server.Id);
            var complaintsChannelId = this._queryBus.Execute(query).ComplaintsChannel.ChannelId;
            return complaintsChannelId != 0;
        }

        private async Task SetChannelPermissions(ChannelContext channel, Contexts contexts)
        {
            var readingAndSending = new List<Permission> { Permission.ReadMessages, Permission.SendMessages };

            var everyonePermissions = new ChangedPermissions(allowPermissions: null, denyPermissions: readingAndSending);
            var adminsPermissions = new ChangedPermissions(readingAndSending, null);
            var mutedPermissions = new ChangedPermissions(readingAndSending, new List<Permission> { Permission.ReadMessageHistory });

            var serverRoles = this._usersRolesService.GetRoles(contexts.Server).ToList();
            var everyoneRole = serverRoles.First(x => x.Name == "@everyone");
            var adminRoles = serverRoles.Where(x => x.Permissions.Contains(Permission.ManageGuild));
            var mutedRole = serverRoles.FirstOrDefault(x => x.Name == UsersRolesService.MUTED_ROLE_NAME);

            var setAdminPerms = adminRoles.Select(role => this._channelsService.SetPermissions(channel, contexts.Server, adminsPermissions, role));
            Task.WaitAll(setAdminPerms.ToArray());
            await this._channelsService.SetPermissions(channel, contexts.Server, mutedPermissions, mutedRole);
            await this._channelsService.SetPermissions(channel, contexts.Server, everyonePermissions, everyoneRole);
        }

        private Task NotifyDomainAboutComplaintsChannel(ChannelContext channel, DiscordServerContext server)
        {
            var addComplaintsChannelCommand = new AddComplaintsChannelCommand(channel.Id, server.Id);
            return this._commandBus.ExecuteAsync(addComplaintsChannelCommand);
        }
    }
}
