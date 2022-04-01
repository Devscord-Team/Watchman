using Devscord.DiscordFramework.Commands.Responses;
using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Watchman.Discord.Areas.Responses.Services
{
    public class ResponsesService : IResponsesService //todo use configuration instead
    {
        public IEnumerable<Response> Responses { get; set; }

        private readonly IResponsesCachingService _responsesCachingService;
        private readonly IResponsesParser _responsesParser;

        public ResponsesService(IResponsesCachingService responsesCachingService, IResponsesParser responsesParser)
        {
            this._responsesCachingService = responsesCachingService;
            this._responsesParser = responsesParser;
        }

        public string GetResponse(ulong serverId, Func<IResponsesService, string> getResponse)
        {
            this.Responses = this._responsesCachingService.GetResponses(serverId);
            var response = getResponse.Invoke(this);
            return response;
        }

        public string ProcessResponse(string response, params KeyValuePair<string, string>[] values)
        {
            return this.ProcessResponse(this.GetResponse(response), values);
        }

        public string ProcessResponse(string response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            return this.ProcessResponse(this.GetResponse(response), contexts, values);
        }

        private Response GetResponse(string name)
        {
            return this.Responses.SingleOrDefault(x => x.OnEvent == name);
        }

        private string ProcessResponse(Response response, params KeyValuePair<string, string>[] values)
        {
            if (values.Length != response.GetFields().Count())
            {
                throw new ArgumentException(@"Cannot process response {response}. Values must be equal to required.", response.ToJson());
            }
            return this._responsesParser.Parse(response, values);
        }

        private string ProcessResponse(Response response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            var fields = contexts.ConvertToResponseFields(response.GetFields()).ToList();
            fields.AddRange(values);
            return this._responsesParser.Parse(response, fields);
        }
    }
}
