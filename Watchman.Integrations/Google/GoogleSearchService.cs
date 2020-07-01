using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Watchman.Integrations.Google.Models;

namespace Watchman.Integrations.Google
{
    public class GoogleSearchService
    {
        private readonly HttpClient _client = new HttpClient();
        private readonly HtmlDocument _html = new HtmlDocument();

        public IEnumerable<GoogleSearchResult> Search(string query)
        {
            var url = GetGoogleSearchUrl(query);
            var page = this._client.GetAsync(url).Result.Content.ReadAsStringAsync().Result;
            _html.LoadHtml(page);


            var googleResults = _html.DocumentNode.SelectNodes("//div[@class='g']");
            foreach (var googleResult in googleResults)
            {
                
            }

            return default;
        }

        private string GetGoogleSearchUrl(string query)
        {
            var url = "https://www.google.com/search?q=" + query;
            var encodedUrl = HttpUtility.HtmlEncode(url);
            return encodedUrl;
        }
    }
}
