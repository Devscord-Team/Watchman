using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Parsing.Models;
using Devscord.DiscordFramework.Framework.Commands.Responses.Resources;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Serilog;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Initialization.Services;
using Watchman.DomainModel.Messages;
using Watchman.DomainModel.Messages.Commands;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Initialization.Controllers
{
    public class InitializationController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MuteRoleInitService _muteRoleInitService;
        private readonly UsersRolesService _usersRolesService;
        private readonly ReadMessagesHistoryService _readMessagesHistoryService;

        public InitializationController(IQueryBus queryBus, ICommandBus commandBus, MuteRoleInitService muteRoleInitService, UsersRolesService usersRolesService, ReadMessagesHistoryService readMessagesHistoryService)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._muteRoleInitService = muteRoleInitService;
            this._usersRolesService = usersRolesService;
            _readMessagesHistoryService = readMessagesHistoryService;
        }

        [AdminCommand]
        [DiscordCommand("init")]
        //[IgnoreForHelp] TODO //TODO co to za TODO?
        public void Init(DiscordRequest request, Contexts contexts)
        {
            _ = ResponsesInit();
            _ = MuteRoleInit(contexts);
            _ = ReadMessagesHistory(contexts.Server);
        }

        private async Task ResponsesInit()
        {
            var responsesInBase = GetResponsesFromBase();
            var defaultResponses = GetResponsesFromResources();

            var responsesToAdd = defaultResponses.Where(def => responsesInBase.All(@base => @base.OnEvent != def.OnEvent));

            var command = new AddResponsesCommand(responsesToAdd);
            await _commandBus.ExecuteAsync(command);
            Log.Debug("Responses initialized");
        }

        private IEnumerable<DomainModel.Responses.Response> GetResponsesFromBase()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = _queryBus.Execute(query).Responses;
            return responsesInBase;
        }

        private IEnumerable<DomainModel.Responses.Response> GetResponsesFromResources()
        {
            var defaultResponses = typeof(Responses).GetProperties()
                .Where(x => x.PropertyType.Name == "String")
                .Select(prop =>
                {
                    var onEvent = prop.Name;
                    var message = prop.GetValue(prop)?.ToString();
                    return new DomainModel.Responses.Response(onEvent, message);
                });

            return defaultResponses;
        }

        private async Task MuteRoleInit(Contexts contexts)
        {
            var mutedRole = _usersRolesService.GetRoleByName(UsersRolesService.MUTED_ROLE_NAME, contexts.Server);

            if (mutedRole == null)
            {
                await _muteRoleInitService.InitForServer(contexts);
            }
            Log.Debug("Mute role initialized");
        }

        private async Task ReadMessagesHistory(DiscordServerContext server)
        {
            var messages = (await _readMessagesHistoryService.ReadMessagesAsync(server))
                .Where(x => !x.contexts.Channel.Name.Contains("logs"))
                .Select(x =>
                {
                    var (contexts, request, sentAt) = x;
                    var builder = Message.Create(request.OriginalMessage);
                    builder.WithAuthor(contexts.User.Id, contexts.User.Name);
                    builder.WithChannel(contexts.Channel.Id, contexts.Channel.Name);
                    builder.WithServer(contexts.Server.Id, contexts.Server.Name, contexts.Server.Owner.Id, contexts.Server.Owner.Name);
                    builder.WithSentAtDate(sentAt);
                    return builder.Build();
                });

            var command = new AddMessagesCommand(messages);
            await _commandBus.ExecuteAsync(command);
            Log.Debug("Read messages history");
        }
    }
}
