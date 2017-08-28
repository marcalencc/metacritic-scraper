using System;
using System.Collections.Generic;
using System.Linq;
using MetacriticScraper.Scraper;
using MetacriticScraper.MediaData;
using MetacriticScraper.Errors;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.RequestData
{
    public class SearchRequestItem : RequestItem
    {
        private int m_limit;
        public int Limit 
        {
            get
            {
                return m_limit;
            }
        }

        private int m_offset;
        public int Offset
        {
            get
            {
                return m_offset;
            }
        }

        private int m_sort;
        public int Sort
        {
            get
            {
                return m_sort;
            }
        }

        private string[] m_parameters;
        public string[] Parameters
        {
            get
            {
                return m_parameters;
            }
        }

        public SearchRequestItem (string id, string title, string thirdLevelRequest,
            string[] parameters) : base(id, title, thirdLevelRequest)
        {
            MediaType = Constants.PersonTypeId;
            m_parameters = parameters;
        }

        public override List<UrlResponsePair> Scrape()
        {
            return null;
        }

        public override bool FilterValidUrls()
        {
            return false;
        }

        protected override void SetThirdLevelRequest()
        {
            string param = "filter-options";
            if (m_thirdLevelRequest == "album")
            {
                param = param + "=music";
            }
            else if (m_thirdLevelRequest == "movie")
            {
                param = param + "=movies";
            }
            else if (m_thirdLevelRequest == "tvshow")
            {
                param = param + "=tv";
            }
            else
            {
                throw new InvalidUrlException(@"Category required for ""search"" request");
            }

            param = param + "&sort_options=date&num_items=100";
            Urls = Urls.Select(u => u + "?" + param).ToList();
        }

        public override IMetacriticData Parse(UrlResponsePair urlResponsePair)
        {
            return null;
        }

        private bool IsMediaTypeAvailable(string html)
        {
            if (m_thirdLevelRequest == "album")
            {
                return html.Contains("tab_music");
            }
            else if (m_thirdLevelRequest == "tvshow")
            {
                return html.Contains("tab_tv");
            }
            else if (m_thirdLevelRequest == "movie")
            {
                return html.Contains("tab_movies");
            }

            return false;
        }

        public override bool Equals(IResult obj)
        {
            bool result = false;
            if (base.Equals(obj))
            {
                result = string.Equals(Name, obj.Name, StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }
}
