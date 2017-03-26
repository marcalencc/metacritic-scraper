using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Album : MediaItem
    {
        public Person PrimaryArtist { get; set; }
    }
}