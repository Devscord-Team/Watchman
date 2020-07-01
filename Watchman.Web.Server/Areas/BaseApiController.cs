using Microsoft.AspNetCore.Mvc;

namespace Watchman.Web.Server.Areas
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    public class BaseApiController : ControllerBase
    {

    }
}
