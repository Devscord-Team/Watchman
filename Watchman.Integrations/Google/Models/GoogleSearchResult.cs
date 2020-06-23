using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Integrations.Google.Models
{
    public class GoogleSearchResult
    {
        public string Title { get; private set; }
        public string Description { get; private set; }
        public Uri Url { get; private set; }

        public GoogleSearchResult(string title, string description, Uri url)
        {
            this.Title = title;
            this.Description = description;
            this.Url = url;
        }
    }
}
