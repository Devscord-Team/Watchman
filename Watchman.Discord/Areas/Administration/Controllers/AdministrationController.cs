using Devscord.DiscordFramework.Commons;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Common.Models;
using Watchman.Discord.Areas.Commons;
using Watchman.DomainModel.DiscordServer.Commands;
using Watchman.DomainModel.Messages.Queries;
using Watchman.Discord.Areas.Users.Services;
using Watchman.Discord.Areas.Administration.BotCommands;
using System.Security.Cryptography.X509Certificates;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly RolesService _rolesService;
        private readonly ParserTime _parserTime;

        public AdministrationController(IQueryBus queryBus, UsersService usersService, DirectMessagesService directMessagesService, MessagesServiceFactory messagesServiceFactory, RolesService rolesService, ParserTime parserTime)
        {
            this._queryBus = queryBus;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            this._messagesServiceFactory = messagesServiceFactory;
            this._rolesService = rolesService;
            this._parserTime = parserTime;
        }

        [AdminCommand]
        public async Task ReadUserMessages(MessagesCommand command, Contexts contexts)
        {
            var mention = command.Mention.StartsWith("<@!") && command.Mention.EndsWith('>') 
                ? command.Mention
                : throw new UserDidntMentionAnyUserException();

            var selectedUser = await this._usersService.GetUserByMentionAsync(contexts.Server, mention) ?? throw new UserNotFoundException(mention);

            var messagesService = this._messagesServiceFactory.Create(contexts);
            TimeRange timeRange = null;
            try
            {
                timeRange = this._parserTime.GetPastTimeRange(timeAsString: command.Time, defaultTime: TimeSpan.FromHours(1));
            }
            catch (InvalidArgumentsException)
            {
                await messagesService.SendResponse(x => x.InvalidArguments());
            }
            catch (TimeIsTooBigException)
            {
                await messagesService.SendResponse(x => x.TimeIsTooBig());
            }
            var query = new GetMessagesQuery(contexts.Server.Id, selectedUser.Id)
            {
                SentDate = timeRange
            };
            var messages = this._queryBus.Execute(query).Messages
                .OrderBy(x => x.SentAt)
                .ToList();

            if (messages.Count > 200 && !command.HasForceArgument)
            {
                await messagesService.SendResponse(x => x.NumberOfMessagesIsHuge(messages.Count));
                return;
            }

            if (!messages.Any())
            {
                await messagesService.SendResponse(x => x.UserDidntWriteAnyMessageInThisTime(selectedUser));
                return;
            }

            var header = $"Messages from user {selectedUser} starting at {timeRange.Start}";
            var lines = messages.Select(x => $"{x.SentAt:yyyy-MM-dd HH:mm:ss} {x.Author.Name}: {x.Content.Replace("```", "")}");
            var linesBuilder = new StringBuilder().PrintManyLines(lines.ToArray(), contentStyleBox: true);

            await this._directMessagesService.TrySendMessage(contexts.User.Id, header);
            await this._directMessagesService.TrySendMessage(contexts.User.Id, linesBuilder.ToString(), MessageType.BlockFormatted);

            await messagesService.SendResponse(x => x.SentByDmMessagesOfAskedUser(messages.Count, selectedUser));
        }

        [AdminCommand]
        public async Task SetRoleAsSafe(SetRoleCommand setRoleCommand, Contexts contexts)
        {
            var roles = setRoleCommand.Roles;
            if (roles.Count == 0 || string.IsNullOrWhiteSpace(roles.First()) || !(setRoleCommand.Safe || setRoleCommand.Unsafe))
            {
                throw new NotEnoughArgumentsException();
            }
            var shouldSetToSafe = setRoleCommand.Safe;
            await this._rolesService.SetRolesAsSafe(contexts, roles, shouldSetToSafe);
        }
    }
}
