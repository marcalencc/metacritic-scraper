using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.MediaData
{
    public class Person : IMetacriticData
    {
        private string m_name;
        public string Name
        {
            get
            {
                return m_name;
            }
        }

        private PersonRatingSummary m_ratingsSummary;
        public PersonRatingSummary RatingsSummary
        {
            get
            {
                return m_ratingsSummary;
            }
        }

        private List<CreditMediaItemPair> m_creditMediaPairItems;
        public List<CreditMediaItemPair> CreditMediaPairItems
        {
            get
            {
                return m_creditMediaPairItems;
            }
            set
            {
                m_creditMediaPairItems = value;
                if (m_creditMediaPairItems.Count > 0)
                {
                    m_ratingsSummary = PersonRatingSummary.GetRatingSummary(
                        m_creditMediaPairItems.Select(m => m.Item));
                }
            }
        }

        public Person (string name)
        {
            m_name = name;
        }

        public Person (string name, List<CreditMediaItemPair> creditMediaPairItems)
        {
            m_name = name;
            m_creditMediaPairItems = creditMediaPairItems;
            m_ratingsSummary = PersonRatingSummary.GetRatingSummary(
                creditMediaPairItems.Select(m => m.Item));
        }

        public struct PersonRatingSummary
        {
            public double? HighestRating;
            private double? m_averageRating;
            public double? AverageRating
            {
                get
                {
                    if (m_averageRating == null)
                    {
                        return 0;
                    }
                    return Math.Round((double) m_averageRating, 2);
                }
                set
                {
                    m_averageRating = value;
                }
            }
            public double? LowestRating;
            public int ReviewCount;

            private PersonRatingSummary (IEnumerable<MediaItem> mediaItemList)
            {
                HighestRating = 0;
                m_averageRating = 0;
                LowestRating = 0;
                ReviewCount = 0;

                List<MediaItem> filtered = mediaItemList.Where(x => x.Rating.CriticRating >= 0).ToList();
                if (filtered.Count > 0)
                {
                    HighestRating = mediaItemList.Max(x => x.Rating.CriticRating);
                    m_averageRating = filtered.Average(x => x.Rating.CriticRating);
                    LowestRating = filtered.Min(x => x.Rating.CriticRating);
                    ReviewCount = filtered.Count();
                }
            }

            public static PersonRatingSummary GetRatingSummary(IEnumerable<MediaItem> mediaItemList)
            {
                return new PersonRatingSummary(mediaItemList);
            }
        }

        public struct CreditMediaItemPair
        {
            public string Credit;
            public MediaItem Item;

            public CreditMediaItemPair(string credit, MediaItem item)
            {
                Credit = credit;
                Item = item;
            }
        }
    }
}