using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.MediaData;
using Newtonsoft.Json;
using MetacriticScraper.JSONObjects;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.RequestData
{
    public abstract class RequestItem : IScrapable<MediaItem>
    {
        private int m_mediaType;
        protected int MediaType
        {
            get
            {
                return m_mediaType;
            }
            set
            {
                m_mediaType = value;
            }
        }

        public string m_searchString;
        protected string SearchString
        {
            get
            {
                return m_searchString;
            }
            set
            {
                m_searchString = value;
            }
        }

        private string m_releaseYear;
        private List<Result> m_autoResult;
        protected List<Result> AutoResult
        {
            get
            {
                return m_autoResult;
            }
            set
            {
                m_autoResult = value;
            }
        }
 
        private List<MediaItem> m_result;
        protected static WebUtils m_webUtils;

        public List<string> Urls { get; set; }

        public RequestItem()
        {
            m_webUtils = new WebUtils();
        }

        public abstract string Scrape(string url);
        public abstract Task<List<MediaItem>> Parse(string html);

        public async Task<bool> AutoSearch()
        {
            string postData = "search_term=" + m_searchString + "&search_each=1";
            string resp = await m_webUtils.HttpPost(Constants.MetacriticURL + "/" + "autosearch", postData,
                Constants.MetacriticDomain, Constants.MetacriticDomain, 30000);
            var completeData = JsonConvert.DeserializeObject<RootObject>(resp);
            if (completeData != null)
            {
                m_autoResult = completeData.AutoComplete.Results;
            }

            return m_autoResult != null;
        }

        public bool FilterValidUrls()
        {
            List<Result> filteredResult = m_autoResult.Where(r => r.RefTypeId == m_mediaType &&
                string.Equals(r.Name, m_searchString, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrEmpty(m_releaseYear))
            {
                filteredResult = m_autoResult.Where(r => string.Equals(r.ItemDate, m_releaseYear)).ToList();
            }

            Urls = filteredResult.Select(r => r.Url).ToList();
            return Urls.Count > 0;
        }
    }
}
