using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Album : MediaItem, IScrapable<Album>
    {
        private Person m_primaryArtist;

        public string Scrape()
        {
            return string.Empty;
        }

        public Album Parse(string html)
        {
            return null;
        }
    }
}