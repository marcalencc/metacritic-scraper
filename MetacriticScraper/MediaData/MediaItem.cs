using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.MediaData
{
    public class MediaItem
    {
        public string Title { get; set; }
        public List<Credit> Credits { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Rating Rating { get; set; }

        public string Scrape()
        {
            return string.Empty;
        }
    }
}
