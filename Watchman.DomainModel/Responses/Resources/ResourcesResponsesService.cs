using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.DomainModel.Responses.Areas.Administration
{
    public interface IResourcesResponsesService
    {
        IEnumerable<Response> GetResponses(params string[] areas);
    }

    public class ResourcesResponsesService : IResourcesResponsesService
    {
        public IEnumerable<Response> GetResponses(params string[] areas)
        {
            foreach (var area in areas)
            {
                var path = Path.Combine(Environment.CurrentDirectory, "Responses", "Resources", $"{area}DefaultResponses.json");
                var json = File.ReadAllText(path);
                var defaultResponses = JsonConvert.DeserializeObject<List<DefaultResponseModel>>(json);
                foreach (var defaultResponse in defaultResponses)
                {
                    var parameters = new List<string>();
                    if (defaultResponse.Variables != null)
                    {
                        parameters.AddRange(defaultResponse.Variables);
                    }
                    if (defaultResponse.RequireContext)
                    {
                        parameters.AddRange(new[] { "context_user", "context_channel", "context_server" });
                    }
                    yield return new Response(defaultResponse.OnEvent, defaultResponse.Value, Response.DEFAULT_SERVER_ID, parameters.ToArray());
                }
            }
        }
    }

    public class DefaultResponseModel
    {
        public string OnEvent { get; set; }
        public string Value { get; set; }
        public string[] Variables { get; set; }
        public bool RequireContext { get; set; }
    }
}
