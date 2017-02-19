using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Interfaces
{
    public interface IScrapable<T>
    {
        List<string> Urls { get; set; }
        string Scrape(string url);
        Task<List<T>> Parse(string html);
    }
}
