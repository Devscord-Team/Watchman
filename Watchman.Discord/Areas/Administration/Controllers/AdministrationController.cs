using Devscord.DiscordFramework.Commons.Exceptions;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Services.Factories;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Administration.Models;
using Watchman.DomainModel.Messages.Queries;
using Devscord.DiscordFramework.Framework.Commands.Responses;

namespace Watchman.Discord.Areas.Administration.Controllers
{
    public class AdministrationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly UsersService _usersService;
        private readonly MessagesServiceFactory _messagesServiceFactory;
        private readonly DirectMessagesService _directMessagesService;

        public AdministrationController(IQueryBus queryBus, UsersService usersService, MessagesServiceFactory messagesServiceFactory, DirectMessagesService directMessagesService)
        {
            this._queryBus = queryBus;
            this._usersService = usersService;
            _messagesServiceFactory = messagesServiceFactory;
            _directMessagesService = directMessagesService;
        }

        [AdminCommand]
        [DiscordCommand("messages")]
        public async Task ReadUserMessages(DiscordRequest request, Contexts contexts)
        {
            var readUserMessagesRequest = new ReadUserMessagesRequest(request.Arguments);
            if(string.IsNullOrWhiteSpace(readUserMessagesRequest.Mention))
            {
                throw new UserNotFoundException(string.Empty);
            }
            var selectedUser = _usersService.GetUsers(contexts.Server).FirstOrDefault(x => x.Mention == readUserMessagesRequest.Mention);
            if(selectedUser == null)
            {
                throw new UserNotFoundException(readUserMessagesRequest.Mention);
            }
            var query = new GetUserMessagesQuery(contexts.Server.Id, selectedUser.Id)
            {
                CreatedDate = readUserMessagesRequest.GetTimeRange()
            };
            var messages = _queryBus.Execute(query).Messages.ToList();

            if (!messages.Any())
            {
                await _directMessagesService.TrySendMessage(contexts.User.Id, "User didnt write any message"); // todo: use response
                //await _directMessagesService.TrySendMessage(contexts.User.Id, x => x.UserDidntWriteAnyMessageInThisTime(scannedUser), contexts);
            }

            var result = new StringBuilder().PrintManyLines(
                header: $"Messages from user {selectedUser} in last {readUserMessagesRequest.MinutesSince} minutes", 
                lines: messages.Select(x => $"{x.CreatedAt:yyyy-MM-dd HH:mm:ss} {x.Author.Name}: {x.Content}").ToArray());

            await _directMessagesService.TrySendMessage(contexts.User.Id, result.ToString());
        }
    }
}
