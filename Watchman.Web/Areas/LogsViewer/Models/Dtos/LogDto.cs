using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TypeGen.Core.TypeAnnotations;

namespace Watchman.Web.Areas.LogsViewer.Models.Dtos
{
    [ExportTsClass(OutputDir = "ClientApp/src/models")]
    public class LogDto
    {
        public DateTime Timestamp { get; set; }
        public string Level { get; set; }
        public string MessageTemplate { get; set; }
        public Dictionary<string, string> Properties { get; set; }
    }
}