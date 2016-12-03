using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class TVShow : MediaItem, IScrapable<TVShow>, IMediaItem
    {
        private int m_season;

        public TVShow Parse(string html)
        {
            return null;
        }
    }
}