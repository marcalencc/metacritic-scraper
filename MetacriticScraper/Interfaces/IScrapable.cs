using System;
using System.Collections.Generic;
using MetacriticScraper.Scraper;

namespace MetacriticScraper.Interfaces
{
    public interface IScrapable<T>
    {
        List<string> Urls { get; set; }
        List<UrlResponsePair> Scrape();
        T Parse(UrlResponsePair urlResponsePair);
        string RequestId { get; }
    }
}
