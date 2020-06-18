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

        public Func<Contexts, IEnumerable<Response>> GetResponsesFunc { get; set; } = x => throw new NotImplementedException();

        public ResponsesService() => this._parser = new ResponsesParser();

        public void RefreshResponses(Contexts contexts) => this.Responses = this.GetResponsesFunc(contexts);

        public Response GetResponse(string name) => this.Responses.SingleOrDefault(x => x.OnEvent == name);

        public string ProcessResponse(string response, params KeyValuePair<string, string>[] values) => this.ProcessResponse(this.GetResponse(response), values);

        public string ProcessResponse(string response, Contexts contexts, params KeyValuePair<string, string>[] values) => this.ProcessResponse(this.GetResponse(response), contexts, values);

        public string ProcessResponse(Response response, params KeyValuePair<string, string>[] values)
        {
            if (values.Length != response.GetFields().Count())
            {
                throw new ArgumentException($"Cannot process response {response.OnEvent}. Values must be equal to required.");
            }
            Log.Debug($"Start parsing response {response} with values {values}");
            return this._parser.Parse(response, values);
        }

        public string ProcessResponse(Response response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            var fields = contexts.ConvertToResponseFields(response.GetFields()).ToList();
            Log.Debug($"Found fields {fields} in response {response}");
            fields.AddRange(values);
            Log.Debug($"Start parsing response {response} with values {values}");
            return this._parser.Parse(response, fields);
        }
    }
}
