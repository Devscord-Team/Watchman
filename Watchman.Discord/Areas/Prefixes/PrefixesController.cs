using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Framework.Architecture.Controllers;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Watchman.Cqrs;
using Watchman.Discord.Areas.Prefixes.Commands;
using Watchman.DomainModel.ServerPrefixes.Commands;
using Watchman.DomainModel.ServerPrefixes.Queries;

namespace Watchman.Discord.Areas.Prefixes
{
    public class PrefixesController : IController
    {
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;
        private readonly MessagesServiceFactory _messagesServiceFactory;

        public PrefixesController(IQueryBus queryBus, ICommandBus commandBus, MessagesServiceFactory messagesServiceFactory)
        {
            this._queryBus = queryBus;
            this._commandBus = commandBus;
            this._messagesServiceFactory = messagesServiceFactory;
        }

        public async Task GetPrefixes(PrefixesCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);

            var query = new GetPrefixesQuery(contexts.Server.Id);
            var prefixes = (await this._queryBus.ExecuteAsync(query)).Prefixes;

            var output = new StringBuilder().PrintManyLines(prefixes.Prefixes.ToArray(), contentStyleBox: false, spacesBetweenLines: false);
            //await messageService.SendResponse(x => x.Response(output.ToString()));
        }

        public async Task AddPrefix(Commands.AddPrefixCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);

            var addPrefixCommand = new DomainModel.ServerPrefixes.Commands.AddPrefixCommand(contexts.Server.Id, command.Prefix);
            await this._commandBus.ExecuteAsync(addPrefixCommand);

            //await messageService.SendResponse(x => x.Response(output.ToString()));
        }

        public async Task RemovePrefix(RemovePrefixCommand command, Contexts contexts)
        {
            var messageService = this._messagesServiceFactory.Create(contexts);

            var deletePrefixCommand = new DeletePrefixCommand(contexts.Server.Id, command.Prefix);
            await this._commandBus.ExecuteAsync(deletePrefixCommand);

            //await messageService.SendResponse(x => x.Response(output.ToString()));
        }
    }
}
