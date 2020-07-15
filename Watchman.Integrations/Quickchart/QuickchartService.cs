using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Watchman.Integrations.Quickchart.Models;

namespace Watchman.Integrations.Quickchart
{
    public class QuickchartService
    {
        private const string baseUrl = "https://quickchart.io/chart";
        private HttpClient httpClient = new HttpClient();

        public async Task<Stream> GetImage(Chart chart)
        {
            var labels = chart.Labels.Select(x => "'" + x + "'").Aggregate((a, b) => a + "," + b);
            var datasetValues = chart.Data.Data.Select(x => x.ToString()).Aggregate((a, b) => a + "," + b);
            var url = baseUrl + $"?c={{type:'{chart.Type}',data:{{labels:[{labels}], datasets:[{{label:'{chart.Data.Label}',data:[{datasetValues}]}}]}}}}" + "&backgroundColor=white";

            var response = await this.httpClient.GetAsync(url);
            return await response.Content.ReadAsStreamAsync();
        }
    }
}
