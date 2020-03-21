using System.Linq;
using System.Net;
using Watchman.Integrations.Quickchart.Models;

namespace Watchman.Integrations.Quickchart
{
    public class QuickchartService
    {
        private const string baseUrl = "https://quickchart.io/chart";

        public string GetImage(Chart chart)
        {
            var labels = chart.Labels.Select(x => "'" + x + "'").Aggregate((a, b) => a + "," + b);
            var datasetValues = chart.Data.Data.Select(x => x.ToString()).Aggregate((a, b) => a + "," + b);
            var url = baseUrl + $"?c={{type:'{chart.Type}',data:{{labels:[{labels}], datasets:[{{label:'{chart.Data.Label}',data:[{datasetValues}]}}]}}}}" + "&backgroundColor=white";

            using var client = new WebClient();
            client.DownloadFile(url, "statistics.png");
            return "statistics.png";
        }
    }
}
