using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Commands.Responses
{
    public class ResponsesService
    {
        public IEnumerable<Response> Responses { get; set; }

        private readonly ResponsesCachingService _responsesCachingService;
        private readonly ResponsesParser _responsesParser;

        public ResponsesService(ResponsesCachingService responsesCachingService, ResponsesParser responsesParser)
        {
            this._responsesCachingService = responsesCachingService;
            this._responsesParser = responsesParser;
        }

        public string GetResponse(ulong serverId, Func<ResponsesService, string> getResponse)
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
            Log.Debug("Start parsing response {response} with values {values}", response.ToJson(), values.ToJson());
            return this._responsesParser.Parse(response, values);
        }

        private string ProcessResponse(Response response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            var fields = contexts.ConvertToResponseFields(response.GetFields()).ToList();
            Log.Debug("Found fields {fields} in response {response}", fields.ToJson(), response.ToJson());
            fields.AddRange(values);
            Log.Debug("Start parsing response {response} with values {values}", response.ToJson(), values.ToJson());
            return this._responsesParser.Parse(response, fields);
        }
    }
}
