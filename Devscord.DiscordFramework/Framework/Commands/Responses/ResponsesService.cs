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
    public class ResponsesService : IService
    {
        public IEnumerable<Response> Responses { get; set; }
        private readonly ResponsesParser parser;
        public ResponsesService()
        {
            string filePathForVS;
            string filePathForCLI;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                filePathForVS = @"Framework\\Commands\\Responses\\responses-configuration.json";
                filePathForCLI = @"bin\\Debug\\netcoreapp3.0\\Framework\\Commands\\Responses\\responses-configuration.json";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
            {
                filePathForVS = @"Framework/Commands/Responses/responses-configuration.json";
                filePathForCLI = @"bin/Debug/netcoreapp3.0/Framework/Commands/Responses/responses-configuration.json";
            }
            else
            {
                throw new Exception("Unrecognized operating system.");
            }

            string allText;

            if (File.Exists(filePathForVS))
                allText = File.ReadAllText(filePathForVS);
            else
                allText = File.ReadAllText(filePathForCLI);

            this.Responses = JsonConvert.DeserializeObject<IEnumerable<Response>>(allText);
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

        public string ProcessResponse(string response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            return ProcessResponse(GetResponse(response), contexts, values);
        }

        public string ProcessResponse(Response response, params KeyValuePair<string, string>[] values)
        {
            if(values.Count() != response.GetFields().Count())
            {
                throw new ArgumentException($"Cannot process response {response.OnEvent}. Values must be equal to required.");
            }
            return parser.Parse(response, values);
        }

        public string ProcessResponse(Response response, Contexts contexts, params KeyValuePair<string, string>[] values)
        {
            var fields = contexts.ConvertToResponseFields(response.GetFields()).ToList();
            fields.AddRange(values);
            return parser.Parse(response, fields);
        }
    }
}
