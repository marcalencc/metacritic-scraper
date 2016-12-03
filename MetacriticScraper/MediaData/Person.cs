using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Person : IScrapable<Person>
    {
        private string m_name;
        private List<MediaItem> m_tvShows;
        private List<MediaItem> m_films;
        private List<MediaItem> m_albums;
        private PersonRatingSummary m_tvShowRatingsSummary;
        private PersonRatingSummary m_filmRatingsSummary;
        private PersonRatingSummary m_albumRatingsSummary;

        public Person (string name, List<MediaItem> tvShows, List<MediaItem> films,
            List<MediaItem> albums)
        {
            m_name = name;
            m_tvShows = tvShows;
            m_films = films;
            m_albums = albums;
            m_tvShowRatingsSummary = PersonRatingSummary.GetRatingSummary(tvShows);
            m_filmRatingsSummary = PersonRatingSummary.GetRatingSummary(films);
            m_albumRatingsSummary = PersonRatingSummary.GetRatingSummary(albums);
        }

        public string Scrape()
        {
            return string.Empty;
        }

        public Person Parse(string html)
        {
            return null;
        }

        private struct PersonRatingSummary
        {
            public double HighestRating;
            public double AverageRating;
            public double LowestRating;
            public int ReviewCount;

            private PersonRatingSummary (List<MediaItem> mediaItemList)
            {
                this.HighestRating = mediaItemList.Max(x => x.Rating.CriticRating);
                this.AverageRating = mediaItemList.Average(x => x.Rating.CriticRating);
                this.LowestRating = mediaItemList.Min(x => x.Rating.CriticRating);
                this.ReviewCount = mediaItemList.Count();
            }

            public static PersonRatingSummary GetRatingSummary(List<MediaItem> mediaItemList)
            {
                return new PersonRatingSummary(mediaItemList);
            }
        }
    }
}