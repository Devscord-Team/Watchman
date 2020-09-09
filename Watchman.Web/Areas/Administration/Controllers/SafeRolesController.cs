using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Cqrs;

namespace Watchman.Web.Areas.Administration.Controllers
{
    public class SafeRolesController : BaseApiController
    {
        private readonly IQueryBus _queryBus;

        public SafeRolesController(IQueryBus queryBus)
        {
            this._queryBus = queryBus;
        }
    }
}
