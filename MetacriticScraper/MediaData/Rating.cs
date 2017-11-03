using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.MediaData
{
    public class Rating
    {
        public Rating(short? criticRating, float? userRating,
            short? criticReviewCount, short? userReviewCount)
        {
            CriticRating = criticRating;
            UserRating = userRating;
            CriticReviewCount = criticReviewCount;
            UserReviewCount = userReviewCount;
        }

        public Rating(short? criticRating) :
            this(criticRating, null, null, null)
        {
        }

        public Rating (short? criticRating, short? criticReviewCount):
            this (criticRating, null, criticReviewCount, null)
        {
        }

        public Rating(short? criticRating, float? userRating) :
            this(criticRating, userRating, null, null)
        {
        }

        public short? CriticRating { get; set; }
        public short? CriticReviewCount { get; set; }
        public float? UserRating { get; set; }
        public short? UserReviewCount { get; set; }
    }
}
