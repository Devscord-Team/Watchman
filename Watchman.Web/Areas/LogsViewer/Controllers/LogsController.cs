using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Watchman.Web.Areas.LogsViewer.Models.Dtos;
using Watchman.Web.Areas.LogsViewer.Services;

namespace Watchman.Web.Areas.LogsViewer.Controllers
{
    public class LogsController : BaseApiController
    {
        private readonly LogsService logsService;

        public LogsController(LogsService logsService) => this.logsService = logsService;

        [HttpGet]
        public IEnumerable<LogDto> GetLogs([FromQuery]GetLogsRequest request = null)
        {
            if (request.Limit > 200)
            {
                request.Limit = 200;
            }

            return this.logsService.GetLogs(request);
        }
    }
}
