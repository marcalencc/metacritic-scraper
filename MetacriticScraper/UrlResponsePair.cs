using System;

namespace MetacriticScraper.Scraper
{
    public struct UrlResponsePair
    {
        public string Url;
        public string Response;

        public UrlResponsePair(string url, string response)
        {
            Url = url;
            Response = response;
        }
    }
}
