using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.RequestData
{
    public class MovieRequest : IMediaItemRequest
    {
        private string m_searchString;
        public string SearchString
        {
            get
            {
                return m_searchString;
            }
        }

        private string m_releaseYear;
        public string ReleaseYear
        {
            get
            {
                return m_releaseYear;
            }
        }

        public MovieRequest(string url)
        {
            m_searchString = "Moonlight";
        }
    }
}
