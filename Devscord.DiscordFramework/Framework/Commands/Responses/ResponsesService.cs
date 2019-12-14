using Devscord.DiscordFramework.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Devscord.DiscordFramework.Framework.Commands.Responses
{
    public class ResponsesService : IService
    {
        public IEnumerable<Response> Responses { get; set; }
        public ResponsesService()
        {
            this.Responses = JsonConvert.DeserializeObject<IEnumerable<Response>>(File.ReadAllText("responses-configuration.json"));
        }


    }
}
