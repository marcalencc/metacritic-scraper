using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class MediaItem : IMetacriticData, IEquatable<MediaItem>
    {
        public string Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Rating Rating { get; set; }
        public string ImageUrl { get; set; }

        public bool Equals(MediaItem other)
        {
            return Title == other.Title &&
                ReleaseDate == other.ReleaseDate;
        }
    }
}
