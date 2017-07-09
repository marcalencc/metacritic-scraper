using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.MediaData;
using Newtonsoft.Json;
using MetacriticScraper.JSONObjects;
using MetacriticScraper.Interfaces;
using MetacriticScraper.Web;
using NLog;

namespace MetacriticScraper.RequestData
{
    public abstract class RequestItem : IScrapable<MediaItem>, IResult, IEquatable<IResult>
    {
        protected static Logger Logger = LogManager.GetCurrentClassLogger();

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
        public List<Result> AutoResult
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
        public List<string> Urls { get; set; }

        protected static IWebUtils m_webUtils;
        public IWebUtils WebUtils
        {
            get
            {
                return m_webUtils;
            }
            set
            {
                m_webUtils = value;
            }
        }

        private string m_requestId;
        public string RequestId
        {
            get
            {
                return m_requestId;
            }
        }

        private string m_thirdLevelRequest;
        public string ThirdLevelRequest
        {
            get
            {
                return m_thirdLevelRequest;
            }
        }

        protected RequestItem(string id, string searchString, string thirdLevelReq)
        {
            m_searchString = searchString.Replace("-", " ");
            m_requestId = id;
            Logger = LogManager.GetLogger(GetType().FullName);
            m_webUtils = new WebUtils();
            m_thirdLevelRequest = thirdLevelReq;
        }

        protected RequestItem(string id, string searchString, string releaseYear, string thirdLevelReq) :
            this(id, searchString, thirdLevelReq)
        {
            m_releaseYear = releaseYear;
        }

        public abstract List<string> Scrape();
        public abstract MediaItem Parse(string html);
        public abstract bool FilterValidUrls();

        public async Task<bool> AutoSearch()
        {
            Logger.Info("Autosearching for term {0} ", m_searchString);
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

        public virtual bool Equals(IResult obj)
        {
            return obj != null && obj.RefTypeId == RefTypeId;
        }
        #endregion IResult
    }
}
