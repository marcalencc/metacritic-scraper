using System;

namespace MetacriticScraper.Scraper
{
    public struct UrlResponsePair
    {
        public string Url;
        public string Response;
        public int SequenceNo;
        public int SearchItemCount;

        public UrlResponsePair(string url, string response) :
            this (url, response, -1, -1)
        {
            Url = url;
            Response = response;
        }

        public UrlResponsePair(string url, string response,
            int sequenceNo, int searchItemCount)
        {
            Url = url;
            Response = response;
            SequenceNo = sequenceNo;
            SearchItemCount = searchItemCount;
        }
    }
}
