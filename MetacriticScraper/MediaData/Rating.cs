using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetacriticScraper.MediaData
{
    public class Rating
    {
        private short m_criticRating;
        public short CriticRating
        {
            get
            {
                return m_criticRating;
            }
        }

        private short m_criticReviewCount;

        private short m_userRating;

        private short m_userReviewCount;
    }
}
