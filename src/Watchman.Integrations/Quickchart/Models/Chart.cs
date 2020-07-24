using System.Collections.Generic;

namespace Watchman.Integrations.Quickchart.Models
{
    public class Chart
    {
        public string Type { get; set; }
        public IEnumerable<string> Labels { get; set; }
        public Dataset Data { get; set; }
    }

    public class Dataset
    {
        public string Label { get; set; }
        public IEnumerable<int> Data { get; set; }
    }
}
