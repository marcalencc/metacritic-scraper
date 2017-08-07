using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Scraper;

namespace MetacriticScraper.Interfaces
{
    public interface IScraper
    {
        bool AddItem(string id, string url);
        IParser UrlParser { get; set; }
    }
}
