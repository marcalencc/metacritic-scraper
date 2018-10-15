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
            if (!int.TryParse(limitStr, out int limit) || limit > Constants.MAX_LIMIT)
            {
                limit = Constants.DEFAULT_LIMIT;
            }

            string offsetStr = m_parameterData.GetParameterValue("offset");
            int.TryParse(offsetStr, out int offset);

            int lowerLimit = (offset - 1) / 20;
            int remainder = 0;
            if((offset - 1) % 20 != 0)
            {
                remainder = 20 % ((offset - 1) % 20);
            }

            int upperLimit = lowerLimit + (limit - 1 - remainder) / 20;

            if (offset - 1 + limit > 20 * (upperLimit + 1))
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

            int remainder = 0;
            string limitStr = m_parameterData.GetParameterValue("limit");
            if (!int.TryParse(limitStr, out int limit) || limit > Constants.MAX_LIMIT)
            {
                limit = Constants.DEFAULT_LIMIT;
            }

            string offsetStr = m_parameterData.GetParameterValue("offset");
            int.TryParse(offsetStr, out int offset);

            for (int idx = 0; idx < Urls.Count; ++idx)
            {
                string url = Urls[idx];
                int included = limit;

                if (idx == 0)
                {
                    if (offset != 0)
                    {
                        int rem = 20 - ((offset - 1) % 20);
                        included = (limit < rem) ? limit : rem;
                    }
                    remainder = included;
                }
                else if (idx == Urls.Count - 1)
                {
                    included = (limit - remainder) % 20;
                    if(included == 0)
                    {
                        included = 20;
                    }
                }

                var task = m_webUtils.HttpGet(Constants.MetacriticURL + "/" + url,
                    Constants.MetacriticURL + "/search/popular", 30000);
                responses.Add(new UrlResponsePair(url, task.Result, idx + 1, included));
            }

            return responses;
        }

        public override IMetacriticData Parse(UrlResponsePair urlResponsePair)
        {
            SearchData data = new SearchData();
            string response = urlResponsePair.Response;

            if (!string.IsNullOrEmpty(response))
            {
                int lowerBound = 0;
                int upperBound = 20;

                if (Urls.Count > 1)
                {
                    // First in sequence
                    if (urlResponsePair.SequenceNo == 1)
                    {
                        lowerBound = 20 - urlResponsePair.SearchItemCount;
                    }
                    // Last in sequence
                    else if (urlResponsePair.SequenceNo == Urls.Count)
                    {
                        upperBound = urlResponsePair.SearchItemCount;
                    }
                }
                else
                {
                    string offsetStr = m_parameterData.GetParameterValue("offset");
                    int.TryParse(offsetStr, out int offset);
                    int pageAdjustedOffset = 0;
                    if (offset > 0)
                    {
                        pageAdjustedOffset = (offset % 20) - 1;
                    }

                    lowerBound = pageAdjustedOffset;
                    upperBound = pageAdjustedOffset + urlResponsePair.SearchItemCount;
                }

                if (urlResponsePair.SequenceNo == 1)
                {
                    int idx = response.IndexOf(@"class=""query_results""");
                    if (idx >= 0)
                    {
                        response = response.Substring(idx);
                        if (response.Contains(@"Showing 1 to 20 of "))
                        {
                            data.TotalResultCount = int.Parse(ParseItem(ref response, @" of ", @" results"));
                        }
                    }
                    // No result
                    else
                    {
                        return data;
                    }
                }

                int startIdx = response.IndexOf(@"class=""search_results");
                if (startIdx != -1)
                {
                    response = response.Substring(startIdx);
                    int idx = 1;
                    while (response.Contains(@"class=""result_wrap""") && idx <= upperBound)
                    {
                        SearchData.SearchItem item = new SearchData.SearchItem();

                        response = response.Substring(response.IndexOf(@"<span class=""metascore_w"));
                        string criticScoreStr = ParseItem(ref response, @""">", "</span>");
                        short.TryParse(criticScoreStr, out short criticScore);
                        item.Rating = new Rating(criticScore);

                        string id = ParseItem(ref response, @"href=""", @""">");

                        // Album url has different format
                        string[] parts = id.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length > 0 && parts[0] == "music")
                        {
                            id = TrimAlbumUrl(id);
                        }
                        else if (parts.Length > 0 && parts[0] == "tv")
                        {
                            id = id.Replace("tv/", "tvshow/");
                        }
                        item.Id = id;

                        item.Title = ParseItem(ref response, @""">", "</a>");

                        if (idx > lowerBound)
                        {
                            data.AddItem(item);
                        }

                        idx++;
                    }
                }
            }

            return data;
        }

        private string TrimAlbumUrl(string url)
        {
            return url.Substring(0, url.LastIndexOf('/')).Replace("music/", "album/");
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
