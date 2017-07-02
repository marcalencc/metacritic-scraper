using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.RequestData;
using MetacriticScraper.Errors;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.Scraper
{
    public class UrlParser : IParser
    {
        private string[] MAIN_KEYWORDS = new string[] { "/movie/", "/album/", "/tvshow/", "/person/" };

        public bool ParseRequestUrl(string id, string url, out string keyword, out string title,
            out string yearOrSeason)
        {
            try
            {
                keyword = string.Empty;
                for (int idx = 0; idx < MAIN_KEYWORDS.Length; ++idx)
                {
                    if (url.StartsWith(MAIN_KEYWORDS[idx]))
                    {
                        keyword = MAIN_KEYWORDS[idx];
                        break;
                    }
                }

                title = string.Empty;
                yearOrSeason = string.Empty;
                if (!string.IsNullOrEmpty(keyword))
                {
                    url = url.Replace(keyword, string.Empty);
                    if (!string.IsNullOrEmpty(url))
                    {
                        title = url;
                        int slashIdx = url.IndexOf('/');
                        if (slashIdx >= 0)
                        {
                            title = url.Substring(0, slashIdx);
                            url = url.Replace(title + "/", string.Empty);
                            int param;
                            if (!int.TryParse(url, out param))
                            {
                                throw new InvalidUrlException("Invalid year or season value");
                            }
                            else
                            {
                                yearOrSeason = param.ToString();
                            }
                            return true;
                        }
                    }
                }
            }
            catch (InvalidUrlException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new InvalidUrlException("Url has invalid format", ex);
            }
            return false;
        }

        public RequestItem CreateRequestItem(string id, string keyword, string title, string yearOrSeason)
        {
            if (keyword == "/movie/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason) && yearOrSeason.Length == 4)
                {
                    return new MovieRequestItem(id, title, yearOrSeason);
                }
                else
                {
                    return new MovieRequestItem(id, title);
                }
            }
            else if (keyword == "/album/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason) && yearOrSeason.Length == 4)
                {
                    return new AlbumRequestItem(id, title, yearOrSeason);
                }
                else
                {
                    return new AlbumRequestItem(id, title);
                }
            }
            else if (keyword == "/tvshow/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason))
                {
                    return new TVShowRequestItem(id, title, yearOrSeason);
                }
                else
                {
                    return new TVShowRequestItem(id, title);
                }
            }

            return null;
        }
    }
}
