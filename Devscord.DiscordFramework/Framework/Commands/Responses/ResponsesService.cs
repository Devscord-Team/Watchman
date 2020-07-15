using Devscord.DiscordFramework.Commons.Extensions;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class ResponsesService
    {
        public IEnumerable<Response> Responses { get; set; }
        private readonly ResponsesParser _parser;

        public Func<ulong, IEnumerable<Response>> GetResponsesFunc { get; set; } = x => throw new NotImplementedException();

        public ResponsesService()
        {
            this._parser = new ResponsesParser();
        }

        public void RefreshResponses(ulong serverId)
        {
            this.Responses = this.GetResponsesFunc(serverId);
        }

        public Response GetResponse(string name)
        {
            return this.Responses.SingleOrDefault(x => x.OnEvent == name);
        }

        public string ProcessResponse(string response, params KeyValuePair<string, string>[] values)
        {
            return this.ProcessResponse(this.GetResponse(response), values);
        }

        public string ProcessResponse(string response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            return this.ProcessResponse(this.GetResponse(response), contexts, values);
        }

        public string ProcessResponse(Response response, params KeyValuePair<string, string>[] values)
        {
            if (values.Length != response.GetFields().Count())
            {
                throw new ArgumentException("Cannot process response {response}. Values must be equal to required.", response.ToJson());
            }
            Log.Debug("Start parsing response {response} with values {values}", response.ToJson(), values.ToJson());
            return this._parser.Parse(response, values);
        }

        public string ProcessResponse(Response response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            var fields = contexts.ConvertToResponseFields(response.GetFields()).ToList();
            Log.Debug("Found fields {fields} in response {response}", fields, response);
            fields.AddRange(values);
            Log.Debug("Start parsing response {response} with values {values}", response.ToJson(), values.ToJson());
            return this._parser.Parse(response, fields);
        }
    }
}
