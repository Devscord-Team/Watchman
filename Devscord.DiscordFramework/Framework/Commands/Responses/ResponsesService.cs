using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class ResponsesService
    {
        public IEnumerable<Response> Responses { get; set; }
        private readonly ResponsesParser _parser;

        public Func<Contexts, IEnumerable<Response>> GetResponsesFunc { get; set; } = x => throw new NotImplementedException();

        public ResponsesService()
        {
            this._parser = new ResponsesParser();
        }

        public void RefreshResponses(Contexts contexts)
        {
            this.Responses = this.GetResponsesFunc(contexts);
        }

        public Response GetResponse(string name)
        {
            return Responses.SingleOrDefault(x => x.OnEvent == name);
        }

        public string ProcessResponse(string response, params KeyValuePair<string, string>[] values)
        {
            return ProcessResponse(GetResponse(response), values);
        }

        public string ProcessResponse(string response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            return ProcessResponse(GetResponse(response), contexts, values);
        }

        public string ProcessResponse(Response response, params KeyValuePair<string, string>[] values)
        {
            if(values.Length != response.GetFields().Count())
            {
                throw new ArgumentException($"Cannot process response {response.OnEvent}. Values must be equal to required.");
            }
            return _parser.Parse(response, values);
        }

        public string ProcessResponse(Response response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            var fields = contexts.ConvertToResponseFields(response.GetFields()).ToList();
            fields.AddRange(values);
            return _parser.Parse(response, fields);
        }
    }
}
