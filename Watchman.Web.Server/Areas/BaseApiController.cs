using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Watchman.Web.Server.Areas
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    public class BaseApiController : ControllerBase
    {

    }

    [Route("")]
    public class HomeController : BaseApiController
    {
        [HttpGet]
        [Route("")]
        public ContentResult Index()
        {
            var urls = new Dictionary<string, string>();
            urls.Add("swagger", "Swagger");
            urls.Add("hangfire", "Hangfire");

            var result = urls.Select(x => $@"<a href=""{x.Key}"">{x.Value}</a>").Aggregate((a,b) => a + "<br><br>" + b);
            return base.Content(result, "text/html");
        }
    }
}
