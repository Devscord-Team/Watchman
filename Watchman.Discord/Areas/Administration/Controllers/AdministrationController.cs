using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Common.Models;
using Watchman.DomainModel.Messages.Queries;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.Areas.Administration.BotCommands;
using Watchman.Discord.Areas.Administration.Services;
using Watchman.Discord.Areas.Protection.Strategies;
using Watchman.DomainModel.DiscordServer.Queries;
using Watchman.DomainModel.Configuration.Services;
using Watchman.DomainModel.Configuration.ConfigurationItems;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly RolesService _rolesService;
        private readonly TrustRolesService _trustRolesService;
        private readonly CheckUserSafetyService _checkUserSafetyService;
        private readonly UsersRolesService _usersRolesService;
        private readonly IConfigurationService _configurationService;

        public AdministrationController(IQueryBus queryBus, UsersService usersService, DirectMessagesService directMessagesService, MessagesServiceFactory messagesServiceFactory, RolesService rolesService, TrustRolesService trustRolesService, CheckUserSafetyService checkUserSafetyService, UsersRolesService usersRolesService, IConfigurationService configurationService)
        {
            this._queryBus = queryBus;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._rolesService = rolesService;
            this._trustRolesService = trustRolesService;
            this._checkUserSafetyService = checkUserSafetyService;
            this._usersRolesService = usersRolesService;
            this._configurationService = configurationService;
        }

        public async Task ReadUserMessages(MessagesCommand command, Contexts contexts)
        {
            var selectedUser = await this._usersService.GetUserByIdAsync(contexts.Server, command.User);
            if (selectedUser == null)
            {
                throw new UserNotFoundException(command.User.GetUserMention());
            }
            if (!contexts.User.IsAdmin() && selectedUser.Id != command.User) //allow check own messages for everybody
            {
                throw new NotAdminPermissionsException();
            }
            var timeRange = TimeRange.ToNow(contexts.Message.SentAt - command.Time); //todo: change DateTime.Now to Contexts.SentAt
            var query = new GetMessagesQuery(contexts.Server.Id, userId: selectedUser.Id)
            {
                SentDate = timeRange
            };
            var messages = this._queryBus.Execute(query).Messages
                .OrderBy(x => x.SentAt)
                .ToList();

            var messagesService = this._messagesServiceFactory.Create(contexts);
            var maxNumberOfMessages = this._configurationService.GetConfigurationItem<MaxNumberOfMessagesDisplayedByMessageCommandWithoutForce>(contexts.Server.Id).Value;
            if (messages.Count > maxNumberOfMessages && !command.Force)
            {
                await messagesService.SendResponse(x => x.NumberOfMessagesIsHuge(messages.Count));
                return;
            }

            if (!messages.Any())
            {
                await messagesService.SendResponse(x => x.UserDidntWriteAnyMessageInThisTime(selectedUser));
                return;
            }

            var header = $"Messages from user {selectedUser} starting at {timeRange.Start.ToLocalTimeString()}";
            var lines = messages.Select(x => $"{x.SentAt.ToLocalTimeString()} {x.Author.Name}: {x.Content.Replace("```", "")}");
            var linesBuilder = new StringBuilder().PrintManyLines(lines.ToArray(), contentStyleBox: true);

            await this._directMessagesService.TrySendMessage(contexts.User.Id, header);
            await this._directMessagesService.TrySendMessage(contexts.User.Id, linesBuilder.ToString(), MessageType.NormalText);

            await messagesService.SendResponse(x => x.SentByDmMessagesOfAskedUser(messages.Count, selectedUser));
        }

        [AdminCommand]
        public async Task SetRoleAsSafe(SetRoleCommand command, Contexts contexts)
        {
            if (!command.Safe && !command.Unsafe)
            {
                throw new NotEnoughArgumentsException();
            }
            await this._rolesService.SetRolesAsSafe(contexts, command.Roles, setAsSafe: command.Safe);
        }

        [AdminCommand]
        public async Task SetRoleAsTrusted(TrustCommand trustCommand, Contexts contexts)
        {
            await this._trustRolesService.TrustThisRole(trustCommand.Role, contexts);
        }

        [AdminCommand]
        public async Task SetRoleAsUntrusted(UntrustCommand trustCommand, Contexts contexts)
        {
            await this._trustRolesService.DontTrustThisRole(trustCommand.Role, contexts);
        }

        [AdminCommand]
        public async Task GetSafeUsers(SafeUsersCommand safeUsersCommand, Contexts contexts)
        {
            var safeUsersIds = this._checkUserSafetyService.GetSafeUsersIds(contexts.Server.Id);
            var values = new List<KeyValuePair<string, string>>();
            foreach (var safeUserId in safeUsersIds)
            {
                var user = await this._usersService.GetUserByIdAsync(contexts.Server, safeUserId);
                values.Add(new KeyValuePair<string, string>($"{user.Mention} -", user.JoinedServerAt()?.ToLocalTimeString() ?? "nieznana data"));
            }
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (!values.Any())
            {
                await messagesService.SendEmbedMessage("Zaufani użytkownicy", "Serwer nie posiada zaufanych użytkowników", new Dictionary<string, string>());
                return;
            }
            var safeUsers = new Dictionary<string, Dictionary<string, string>>
            {
                {"Zaufani użytkownicy", new Dictionary<string, string>(values)}
            };
            await messagesService.SendEmbedMessage(
                "Zaufani użytkownicy",
                $"Lista zaufanych użytkowników na serwerze {contexts.Server.Name}",
                safeUsers);
        }

        [AdminCommand]
        public async Task GetTrustedRoles(TrustedRolesCommand trustedRolesCommand, Contexts contexts)
        {
            var query = new GetServerTrustedRolesQuery(contexts.Server.Id);
            var trustedRoles = this._queryBus.Execute(query).TrustedRolesIds.ToList();
            var messagesService = this._messagesServiceFactory.Create(contexts);
            if (!trustedRoles.Any())
            {
                await messagesService.SendResponse(x => x.ServerDoesntHaveAnyTrustedRole());
                return;
            }
            var trustedRolesNames = trustedRoles.Select(x => this._usersRolesService.GetRole(x, contexts.Server.Id));
            await messagesService.SendEmbedMessage(
                "Zaufane role",
                $"Lista zaufanych roli na serwerze {contexts.Server.Name}",
                trustedRolesNames.Select(x => new KeyValuePair<string, string>("Nazwa roli:", x.Name)));
        }
    }
}
