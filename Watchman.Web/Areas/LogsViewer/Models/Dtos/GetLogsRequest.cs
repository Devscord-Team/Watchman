using System;
using System.Collections.Generic;
using TypeGen.Core.TypeAnnotations;

namespace Watchman.Web.Areas.LogsViewer.Models.Dtos
{
    [ExportTsClass(OutputDir = "ClientApp/src/models")]
    public class GetLogsRequest
    {
        public short Limit { get; set; } = 20;
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public string Template { get; set; }
        public IEnumerable<string> AcceptedLevels { get; set; }
        public IEnumerable<string> SearchPhrases { get; set; }
    }
}