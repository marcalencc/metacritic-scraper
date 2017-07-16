using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.MediaData;
using MetacriticScraper.Errors;

namespace MetacriticScraper.RequestData
{
    public class PersonRequestItem : RequestItem
    {
        public PersonRequestItem(string id, string title, string thirdLevelRequest) :
            base(id, title, thirdLevelRequest)
        {
            MediaType = Constants.PersonTypeId;
        }

        public override List<string> Scrape()
        {
            Logger.Info("Scraping {0} urls for {1}", Urls.Count, SearchString);
            List<string> urls = new List<string>();
            foreach (string url in Urls)
            {
                var task = m_webUtils.HttpGet(Constants.MetacriticURL + "/" + url, Constants.MetacriticURL, 30000);
                urls.Add(task.Result);
            }

            return urls;
        }

        public override bool FilterValidUrls()
        {
            Urls = m_autoResult.Where(r => this.Equals(r)).Select(r => r.Url).ToList();
            SetThirdLevelRequest();
            Logger.Info("{0} urls filtered for {1}", Urls.Count, SearchString);
            return Urls.Count > 0;
        }

        protected override void SetThirdLevelRequest()
        {
            if (!String.IsNullOrEmpty(m_thirdLevelRequest))
            {
                Urls = Urls.Select(u => u + "/" + m_thirdLevelRequest).ToList();
            }
            else
            {
                throw new InvalidUrlException("Invalid url format - specify category for person");
            }
        }

        public override MetacriticData Parse(string html)
        {
            return null;
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
