using System;
using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Commons;
using Watchman.Cqrs;
using Watchman.DomainModel.Messages.Queries;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Discord.Areas.Commons;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;
        private readonly DirectMessagesService _directMessagesService;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public AdministrationController(IQueryBus queryBus, UsersService usersService, DirectMessagesService directMessagesService, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._usersService = usersService;
            this._directMessagesService = directMessagesService;
            _messagesServiceFactory = messagesServiceFactory;
        }

        [AdminCommand]
        [DiscordCommand("messages")]
        public async Task ReadUserMessages(DiscordRequest request, Contexts contexts)
        {
            var mention = request.GetMention();
            var selectedUser = _usersService.GetUserByMention(contexts.Server, mention);
            if(selectedUser == null)
            {
                throw new UserNotFoundException(mention);
            }

            var timeRange = request.GetPastTimeRange(defaultTime: TimeSpan.FromHours(1));
            var query = new GetUserMessagesQuery(contexts.Server.Id, selectedUser.Id)
            {
                SentDate = timeRange
            };
            var messages = _queryBus.Execute(query).Messages
                .OrderBy(x => x.SentAt)
                .ToList();

            var messagesService = _messagesServiceFactory.Create(contexts);
            var hasForceArgument = request.HasArgument("force") || request.HasArgument("f");

            if (messages.Count > 200 && !hasForceArgument)
            {
                await messagesService.SendResponse(x => x.NumberOfMessagesIsHuge(messages.Count), contexts);
                return;
            }

            if (!messages.Any())
            {
                await _directMessagesService.TrySendMessage(contexts.User.Id, x => x.UserDidntWriteAnyMessageInThisTime(selectedUser), contexts);
            }
            else
            {
                var header = $"Messages from user {selectedUser} starting at {timeRange.Start}";
                var lines = messages.Select(x => $"{x.SentAt:yyyy-MM-dd HH:mm:ss} {x.Author.Name}: {x.Content.Replace("```", "")}");
                var linesBuilder = new StringBuilder().PrintManyLines(lines.ToArray(), contentStyleBox: true);

                await _directMessagesService.TrySendMessage(contexts.User.Id, header);
                await _directMessagesService.TrySendMessage(contexts.User.Id, linesBuilder.ToString(), MessageType.BlockFormatted);
            }

            await messagesService.SendResponse(x => x.SentByDmMessagesOfAskedUser(messages.Count, selectedUser), contexts);
        }
    }
}
