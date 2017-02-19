using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;
using MetacriticScraper.MediaData;

namespace MetacriticScraper.RequestData
{
    public class MovieRequestItem : RequestItem
    {
        public MovieRequestItem(string url)
        {
            SearchString = "Moonlight";
            MediaType = Constants.MovieTypeId;
        }

        public override string Scrape(string url)
        {
            var task = m_webUtils.HttpGet(Constants.MetacriticURL + "/" + url, Constants.MetacriticDomain,
                Constants.MetacriticDomain, 30000);
            return task.Result;
        }

        public override Task<List<MediaItem>> Parse(string html)
        {
            return null;
        }
    }
}
