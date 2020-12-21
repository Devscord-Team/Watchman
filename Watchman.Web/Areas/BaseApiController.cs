using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Watchman.Web.Areas
{
    //[Authorize]
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]/[action]")]
    public abstract class BaseApiController : ControllerBase
    {
    }
}
