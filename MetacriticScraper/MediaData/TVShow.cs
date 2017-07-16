using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class TVShow : MediaItem, IEquatable<TVShow>
    {
        public int Season { get; set; }
        public string Studio { get; set; }

        public bool Equals(TVShow other)
        {
            return base.Equals(other) &&
                Season == other.Season &&
                Studio == other.Studio;
        }
    }
}