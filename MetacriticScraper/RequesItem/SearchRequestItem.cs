using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MetacriticScraper.Scraper;
using MetacriticScraper.MediaData;
using MetacriticScraper.Errors;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.RequestData
{
    public class SearchRequestItem : RequestItem
    {
        private ParameterData m_parameterData;
        public ParameterData ParameterData
        {
            get
            {
                return m_parameterData;
            }
            set
            {
                m_parameterData = value;
            }
        }

        public SearchRequestItem (string id, string title, string thirdLevelRequest,
            string parameterString) : base(id, title, thirdLevelRequest)
        {
            m_parameterData = new ParameterData(parameterString);
            Urls = new List<string>();
        }

        public new async Task<bool> AutoSearch()
        {
            return await Task.FromResult(true);
        }

        public new void RetrieveImagePath()
        {
        }

        public override bool FilterValidUrls()
        {
            string sort = m_parameterData.GetParameterValue("sort");

            string limitStr = m_parameterData.GetParameterValue("limit");
            if (!int.TryParse(limitStr, out int limit))
            {
                limit = 20;
            }

            string offsetStr = m_parameterData.GetParameterValue("offset");
            if (!int.TryParse(offsetStr, out int offset))
            {
                offset = 0;
            }

            int lowerLimit = (offset - 1) / 20;
            int remainder = 0;
            if((offset - 1) % 20 != 0)
            {
                remainder = 20 % ((offset - 1) % 20);
            }

            int upperLimit = lowerLimit + (limit - 1 - remainder) / 20;

            if (remainder > 0)
            {
                upperLimit++;
            }

            for (int idx = lowerLimit; idx <= upperLimit; ++idx)
            {
                Urls.Add(string.Format(@"search/{0}/{1}/results?page={2}&sort={3}",
                    GetThirdLevelRequest(), SearchString.Replace(" ", "%20"), idx, sort));
            }

            return Urls.Count > 0;
        }

        private string GetThirdLevelRequest()
        {
            string category;
            if (m_thirdLevelRequest == "album")
            {
                category = "album";
            }
            else if (m_thirdLevelRequest == "movie")
            {
                category = "movie";
            }
            else if (m_thirdLevelRequest == "tvshow")
            {
                category = "tv";
            }
            else
            {
                throw new InvalidUrlException(@"Category required for ""search"" request");
            }

            return category;
        }

        public override List<UrlResponsePair> Scrape()
        {
            Logger.Info("Scraping {0} urls for {1}", Urls.Count, SearchString);
            List<UrlResponsePair> responses = new List<UrlResponsePair>();
            foreach (string url in Urls)
            {
                var task = m_webUtils.HttpGet(Constants.MetacriticURL + "/" + url, Constants.MetacriticURL, 30000);
                responses.Add(new UrlResponsePair(url, task.Result));
            }

            return responses;
        }

        public override IMetacriticData Parse(UrlResponsePair urlResponsePair)
        {
            SearchData data = new SearchData();
            string response = urlResponsePair.Response;
            if (!string.IsNullOrEmpty(response))
            {
                int startIdx = response.IndexOf(@"class=""search_results""");
                if (startIdx != -1)
                {
                    response = response.Substring(startIdx);
                    while (response.Contains(@"class=""result_wrap"""))
                    {
                        SearchData.SearchItem item = new SearchData.SearchItem();
                        item.Id = ParseItem(ref response, @"href=""", @""">");
                        item.Title = ParseItem(ref response, @""">", "</a>");
                        string criticScoreStr = ParseItem(ref response, @""">", "</span>");
                        short.TryParse(criticScoreStr, out short criticScore);
                        item.Rating = new Rating(criticScore);
                        item.ReleaseDate = ParseItem(ref response, @"<span class=""data"">", "</span>");

                        if (response.Contains(@"class=""stat genre"""))
                        {
                            response = response.Substring(response.IndexOf(@"class=""stat genre"""));
                            string genre = ParseItem(ref response, @"<span class=""data"">", "</span>");
                            Regex rgx = new Regex("\\s+");
                            item.Genre = rgx.Replace(genre, " ");
                        }

                        data.AddItem(item);
                    }
                }
            }

            return data;
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
