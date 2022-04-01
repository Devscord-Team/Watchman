﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using Watchman.Cqrs;
using Watchman.DomainModel.Responses.Queries;
using Response = Watchman.DomainModel.Responses.Response;

namespace Watchman.Discord.Areas.Responses.Services
{
    public interface IResponsesGetterService
    {
        IEnumerable<Response> GetResponsesFromBase();
        IEnumerable<Response> GetResponsesFromResources();
    }

    public class ResponsesGetterService : IResponsesGetterService
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
            var defaultResponses = typeof(Devscord.DiscordFramework.Commands.Responses.Resources.Responses).GetProperties()
                .Where(x => x.PropertyType.Name == "String")
                .Select(prop =>
                {
                    var onEvent = prop.Name;
                    var message = prop.GetValue(prop)?.ToString();
                    // .Skip(1) as the first argument is of type ResponsesService, which is not a part of available variables list
                    var parameters = managerMethods.First(info => info.Name == onEvent).GetParameters().Skip(1);
                    var availableVariables = new HashSet<string>();
                    foreach (var parameter in parameters)
                    {
                        if (parameter.ParameterType.Name == nameof(Contexts))
                        {
                            availableVariables.Add("user");
                            availableVariables.Add("channel");
                            availableVariables.Add("server");
                        }
                        else
                        {
                            availableVariables.Add(parameter.Name);
                        }
                    }
                    return new Response(onEvent, message, DEFAULT_SERVER_ID, availableVariables.ToArray());
                });
            Log.Information("Default responses initialized.");
            return defaultResponses;
        }
    }
}
