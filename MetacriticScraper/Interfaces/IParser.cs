using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.RequestData;

namespace MetacriticScraper.Interfaces
{
    public interface IParser
    {
        bool ParseRequestUrl(string id, string url, out string keyword, out string title,
            out string yearOrSeason);

        RequestItem CreateRequestItem(string id, string keyword, string title, string yearOrSeason);
    }
}
