using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Person
    {
        private string m_name;
        private List<MediaItem> m_mediaItems;
        private PersonRatingSummary m_ratingsSummary;

        public Person (string name, List<MediaItem> mediaItems)
        {
            m_name = name;
            m_mediaItems = mediaItems;
            m_ratingsSummary = PersonRatingSummary.GetRatingSummary(mediaItems);
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