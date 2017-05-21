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
    public abstract class RequestItem : IScrapable<MediaItem>, IResult
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

        private string m_searchString;
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

        protected List<Result> m_autoResult;
 
        private List<MediaItem> m_result;
        protected static WebUtils m_webUtils;

        public List<string> Urls { get; set; }

        private string m_requestId;
        public string RequestId
        {
            get
            {
                return m_requestId;
            }
        }


        private RequestItem()
        {
            m_webUtils = new WebUtils();
        }

        protected RequestItem(string id, string searchString) : this()
        {
            m_searchString = searchString.Replace("-", " ");
            m_requestId = id;
        }

        protected RequestItem(string id, string searchString, string releaseYear) : this(id, searchString)
        {
            m_releaseYear = releaseYear;
        }

        public abstract List<string> Scrape();
        public abstract MediaItem Parse(string html);

        public async Task<bool> AutoSearch()
        {
            string postData = "search_term=" + m_searchString + "&search_each=1";
            string resp = await m_webUtils.HttpPost(Constants.MetacriticURL + "/" + "autosearch", postData,
                Constants.MetacriticURL, 30000);
            var completeData = JsonConvert.DeserializeObject<RootObject>(resp);
            if (completeData != null)
            {
                m_autoResult = completeData.AutoComplete.Results;
            }

            return m_autoResult != null;
        }

        public abstract bool FilterValidUrls();

        #region IResult
        public string Name
        {
            get
            {
                return m_searchString;
            }
        }

        public int RefTypeId
        {
            get
            {
                return m_mediaType;
            }
        }

        public string ItemDate
        {
            get
            {
                return m_releaseYear;
            }
        }

        public bool Equals(IResult obj)
        {
            bool result = false;
            if (obj == null)
            {
                return false;
            }
            else if (RefTypeId != obj.RefTypeId)
            {
                return false;
            }
            else if (RefTypeId == Constants.MovieTypeId ||
                RefTypeId == Constants.TvShowTypeId)
            {
                result = string.Equals(Name, obj.Name, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(ItemDate, obj.ItemDate);
            }
            else if (RefTypeId == Constants.AlbumTypeId)
            {
                string name = obj.Name.Split('-')[0].Trim();
                result = string.Equals(Name, obj.Name);
            }

            if (result && obj.ItemDate != null)
            {
                result = string.Equals(ItemDate, obj.ItemDate);
            }

            return result;
        }
        #endregion IResult
    }
}
