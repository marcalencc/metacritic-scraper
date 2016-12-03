using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Film : MediaItem, IScrapable<Film>, IMediaItem
    {
        private Person m_director;

        public Film Parse(string html)
        {
            return null;
        }
    }
}