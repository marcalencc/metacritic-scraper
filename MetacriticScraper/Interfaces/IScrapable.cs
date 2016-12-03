using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.Interfaces
{
    public interface IScrapable<T>
    {
        string Scrape();
        T Parse(string html);
    }
}
