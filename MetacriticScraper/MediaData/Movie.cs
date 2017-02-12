using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Movie : MediaItem, IScrapable<Movie>
    {
        private Person m_director;
        public string Scrape()
        {

            return null;
        }

        public Movie Parse(string html)
        {
            return null;
        }
    }
}