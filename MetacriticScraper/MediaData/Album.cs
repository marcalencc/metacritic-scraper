using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Album : MediaItem, IEquatable<Album>
    {
        public string PrimaryArtist { get; set; }

        public bool Equals(Album other)
        {
            return base.Equals(other) && PrimaryArtist == other.PrimaryArtist;
        }
    }
}