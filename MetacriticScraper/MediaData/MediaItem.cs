using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class MediaItem : IMediaItem
    {
        public string Title { get; set; }
        public List<Credit> Credits { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Rating Rating { get; set; }
    }
}
