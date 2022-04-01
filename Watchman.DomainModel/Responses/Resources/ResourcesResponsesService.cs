using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.DomainModel.Responses.Areas.Administration
{
    public class ResourcesResponsesService
    {
        public async Task<IEnumerable<Response>> GetResponses(string area)
        {
            var path = Path.Combine(Environment.CurrentDirectory, "Responses", "Resources", $"{area}DefaultResponses.json");
            var json = await File.ReadAllTextAsync(path);
            var defaultResponses = JsonConvert.DeserializeObject<List<DefaultResponseModel>>(json);
            var responses = defaultResponses
                .Select(defaultResponse =>
                {
                    var parameters = new List<string>();
                    if(defaultResponse.Variables != null)
                    {
                        parameters.AddRange(defaultResponse.Variables);
                    }
                    parameters.AddRange(new[] { "context_user", "context_channel", "context_server" });
                    return new Response(defaultResponse.OnEvent, defaultResponse.Value, Response.DEFAULT_SERVER_ID, parameters.ToArray());
                });
            return responses;
        }
    }

    public class DefaultResponseModel
    {
        public string OnEvent { get; set; }
        public string Value { get; set; }
        public string[] Variables { get; set; }
    }
}
