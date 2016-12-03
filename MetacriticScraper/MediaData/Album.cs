using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Album : MediaItem, IScrapable<Album>, IMediaItem
    {
        private Person m_primaryArtist;

        public Album Parse(string html)
        {
            return null;
        }
    }
}