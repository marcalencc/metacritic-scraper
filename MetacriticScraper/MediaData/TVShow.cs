using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class TVShow : MediaItem
    {
        public int Season { get; set; }
        public string Studio { get; set; }
    }
}