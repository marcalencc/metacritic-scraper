using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Movie : MediaItem, IEquatable<Movie>
    {
        public string Director { get; set; }

        public bool Equals(Movie other)
        {
            return base.Equals(other) && Director == other.Director;
        }
    }
}