using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devscord.DiscordFramework.Framework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;
using Response = Watchman.DomainModel.Responses.Response;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesGetterService
    {
        private const int DEFAULT_SERVER_ID = 0;
        private readonly IQueryBus _queryBus;

        public ResponsesGetterService(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }

        public IEnumerable<Response> GetResponsesFromBase()
        {
            var query = new GetResponsesQuery();
            var responsesInBase = this._queryBus.Execute(query).Responses;
            return responsesInBase;
        }

        public IEnumerable<Response> GetResponsesFromResources()
        {
            Log.Information("Initializing default responses...");
            var managerMethods = typeof(ResponsesManager).GetMethods();
            var defaultResponses = typeof(Devscord.DiscordFramework.Framework.Commands.Responses.Resources.Responses).GetProperties()
                .Where(x => x.PropertyType.Name == "String")
                .Select(prop =>
                {
                    var onEvent = prop.Name;
                    var message = prop.GetValue(prop)?.ToString();
                    // .Skip(1) bo pierwszym argumentem jest ResponsesService, który nie wchodzi w skład listy pól
                    var parameters = managerMethods.First(info => info.Name == onEvent).GetParameters().Skip(1);
                    var availableVariables = new HashSet<string>();
                    foreach (var parameter in parameters)
                    {
                        switch (parameter.ParameterType.Name)
                        {
                            case "Contexts":
                                availableVariables.Add("user");
                                availableVariables.Add("channel");
                                availableVariables.Add("server");
                                break;
                            case "UserContext":
                                availableVariables.Add("user");
                                break;
                            case "DiscordServerContext":
                                availableVariables.Add("server");
                                break;
                            default:
                                availableVariables.Add(parameter.Name);
                                break;
                        }
                    }
                    return new Response(onEvent, message, DEFAULT_SERVER_ID, availableVariables.ToArray());
                });
            Log.Information("Default responses initialized.");
            return defaultResponses;
        }
    }
}
