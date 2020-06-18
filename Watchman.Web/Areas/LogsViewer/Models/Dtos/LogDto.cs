using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Watchman.Web.Areas.LogsViewer.Models.Dtos
{
    public class LogDto
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
} 