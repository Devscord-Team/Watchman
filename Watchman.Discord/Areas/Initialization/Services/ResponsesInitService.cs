using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devscord.DiscordFramework.Framework.Commands.Responses.Resources;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses;
using Watchman.DomainModel.Responses.Commands;
using Watchman.DomainModel.Responses.Queries;

namespace Watchman.Discord.Areas.Initialization.Services
{
    public class ResponsesInitService
    { 
        private const int DEFAULT_SERVER_ID = 0;
        private readonly IQueryBus _queryBus;
        private readonly ICommandBus _commandBus;

        public ResponsesInitService(IQueryBus queryBus, ICommandBus commandBus)
        {
            _queryBus = queryBus;
            _commandBus = commandBus;
        }

        public async Task InitNewResponsesFromResources()
        {
            var responsesInBase = GetResponsesFromBase();
            var defaultResponses = GetResponsesFromResources();
            var responsesToAdd = defaultResponses
                .Where(def => responsesInBase.All(@base => @base.OnEvent != def.OnEvent))
                .ToList();

            await AddNewResponses(responsesToAdd);
        }

        private IEnumerable<Response> GetResponsesFromBase()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = _queryBus.Execute(query).Responses;
            return responsesInBase;
        }

        private IEnumerable<Response> GetResponsesFromResources()
        {
            var defaultResponses = typeof(Devscord.DiscordFramework.Framework.Commands.Responses.Resources.Responses).GetProperties()
                .Where(x => x.PropertyType.Name == "String")
                .Select(prop =>
                {
                    var onEvent = prop.Name;
                    var message = prop.GetValue(prop)?.ToString();
                    return new DomainModel.Responses.Response(onEvent, message, DEFAULT_SERVER_ID);
                })
                .ToList();

            return defaultResponses;
        }

        private async Task AddNewResponses(IReadOnlyCollection<Response> responsesToAdd)
        {
            if (!responsesToAdd.Any())
            {
                Log.Information("No new responses");
                return;
            }

            var command = new AddResponsesCommand(responsesToAdd);
            await _commandBus.ExecuteAsync(command);
            Log.Information("Responses initialized");
        }
    }
}
