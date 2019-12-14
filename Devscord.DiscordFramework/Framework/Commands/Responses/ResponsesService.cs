using Devscord.DiscordFramework.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class ResponsesService : IService
    {
        public IEnumerable<Response> Responses { get; set; }
        private readonly ResponsesParser parser;
        public ResponsesService()
        {
            this.Responses = JsonConvert.DeserializeObject<IEnumerable<Response>>(File.ReadAllText(@"Framework\Commands\Responses\responses-configuration.json"));
            this.parser = new ResponsesParser();
        }

        public Response GetResponse(string name)
        {
            return Responses.SingleOrDefault(x => x.OnEvent == name);
        }

        public string ProcessResponse(string response, params KeyValuePair<string, string>[] values)
        {
            return ProcessResponse(GetResponse(response), values);
        }

        public string ProcessResponse(Response response, params KeyValuePair<string, string>[] values)
        {
            if(values.Count() == 0)
            {
                throw new ArgumentException($"Cannot process response {response.OnEvent}. Values must be set.");
            }
            return parser.Parse(response, values);
        }
    }
}
