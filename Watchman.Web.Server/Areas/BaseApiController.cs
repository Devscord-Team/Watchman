using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Server.Areas
{
    [ApiController]
    [Produces("application/json")]
    [Route("[controller]/[action]")]
    //[EnableCors("AllowAny")]
    public class BaseApiController : ControllerBase
    {
    }
}
