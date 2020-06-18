using System;
using System.Collections.Generic;

namespace Watchman.Web.Areas.LogsViewer.Models.Dtos
{
    public class GetLogsRequest
    {
        public short? Limit { get; set; } = 20;
        public DateTime? FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public string Level { get; set; }
        public IEnumerable<KeyValuePair<string, string>> PropertiesRequirements { get; set; }
    }
}