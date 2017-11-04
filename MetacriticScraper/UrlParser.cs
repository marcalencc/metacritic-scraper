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
        private string[] MAIN_KEYWORDS = new string[] { "/movie/", "/album/", "/tvshow/", "/person/", "/search/" };
        private string[] OTHER_KEYWORDS = new string[] { "details" };
        private string[] MEDIA_TYPES = new string[] {"movie", "album", "tvshow"};
        private string[] SEARCH_TYPES = new string[] { "movie", "album", "tvshow", "person" };

        public bool ParseRequestUrl(string id, string url, out string keyword, out string title,
            out string yearOrSeason, out string thirdLevelReq)
        {
            string parameters = null;
            return ParseRequestUrl(id, url, out keyword, out title, out yearOrSeason, out thirdLevelReq,
                ref parameters);
        }

        public bool ParseRequestUrl(string id, string url, out string keyword, out string title,
            out string yearOrSeason, out string thirdLevelReq, ref string parameters)
        {
            try
            {
                if (url.EndsWith("/"))
                {
                    url = url.Remove(url.Length - 1);
                }

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
                thirdLevelReq = string.Empty;
                if (!string.IsNullOrEmpty(keyword))
                {
                    url = url.Replace(keyword, string.Empty);
                    if (!string.IsNullOrEmpty(url))
                    {
                        title = url;
                        int slashIdx = url.IndexOf('/');
                        if (slashIdx > 0)
                        {
                            title = url.Substring(0, slashIdx);
                            url = url.Replace(title + "/", string.Empty);
                            slashIdx = url.IndexOf('/');

                            // There is only either year or third level request
                            if (slashIdx < 0)
                            {
                                if (keyword.Contains("person"))
                                {
                                    if (MEDIA_TYPES.Contains(url))
                                    {
                                        thirdLevelReq = url;
                                    }
                                    else
                                    {
                                        throw new InvalidUrlException("Invalid parameter: " + url);
                                    }
                                }
                                else if (keyword.Contains("search"))
                                {
                                    if (title.Length <= 3)
                                    {
                                        throw new InvalidUrlException("Search query should be at least three characters.");
                                    }

                                    thirdLevelReq = url;
                                    if (url.IndexOf("?") >= 0)
                                    {
                                        thirdLevelReq = url.Substring(0, url.IndexOf("?"));
                                    }

                                    if (SEARCH_TYPES.Contains(thirdLevelReq))
                                    {
                                        url = url.Replace(thirdLevelReq + "?", "");
                                        parameters = url;
                                    }
                                    else
                                    {
                                        throw new InvalidUrlException("Invalid parameter: " + url);
                                    }
                                }
                                else
                                {
                                    int param;
                                    if (!int.TryParse(url, out param))
                                    {
                                        if (OTHER_KEYWORDS.Contains(url))
                                        {
                                            thirdLevelReq = url;
                                        }
                                        else
                                        {
                                            throw new InvalidUrlException("Invalid parameter: " + url);
                                        }
                                    }
                                    else
                                    {
                                        yearOrSeason = param.ToString();
                                    }
                                }
                            }
                            else
                            {
                                string yearString = url.Substring(0, slashIdx);
                                int param;
                                if (!int.TryParse(yearString, out param))
                                {
                                    throw new InvalidUrlException("Invalid year or season value");
                                }
                                else
                                {
                                    yearOrSeason = param.ToString();
                                    url = url.Replace(param.ToString() + "/", string.Empty);
                                    if (OTHER_KEYWORDS.Contains(url))
                                    {
                                        thirdLevelReq = url;
                                    }
                                    else
                                    {
                                        throw new InvalidUrlException("Invalid parameter: " + url);
                                    }
                                }
                            }
                            return true;
                        }
                        else if (keyword.Contains("person") || keyword.Contains("search"))
                        {
                            throw new InvalidUrlException(@"Category required for """ +
                                keyword.Replace("/", "") + @""" request");
                        }
                        return true;
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

        public RequestItem CreateRequestItem(string id, string keyword, string title, string yearOrSeason,
            string thirdLevelReq)
        {
            return CreateRequestItem(id, keyword, title, yearOrSeason, thirdLevelReq, null);
        }

        public RequestItem CreateRequestItem(string id, string keyword, string title, string yearOrSeason,
            string thirdLevelReq, string parameterString)
        {
            if (keyword == "/movie/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason) && yearOrSeason.Length == 4)
                {
                    return new MovieRequestItem(id, title, yearOrSeason, thirdLevelReq);
                }
                else
                {
                    return new MovieRequestItem(id, title, thirdLevelReq);
                }
            }
            else if (keyword == "/album/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason) && yearOrSeason.Length == 4)
                {
                    return new AlbumRequestItem(id, title, yearOrSeason, thirdLevelReq);
                }
                else
                {
                    return new AlbumRequestItem(id, title, thirdLevelReq);
                }
            }
            else if (keyword == "/tvshow/")
            {
                if (!string.IsNullOrEmpty(yearOrSeason))
                {
                    return new TVShowRequestItem(id, title, yearOrSeason, thirdLevelReq);
                }
                else
                {
                    return new TVShowRequestItem(id, title, thirdLevelReq);
                }
            }
            else if (keyword == "/person/")
            {
                if (string.IsNullOrEmpty(thirdLevelReq))
                {
                    throw new InvalidUrlException(@"Category required for ""person"" request");
                }
                else
                {
                    return new PersonRequestItem(id, title, thirdLevelReq);
                }
            }
            else if (keyword == "/search/")
            {
                if (string.IsNullOrEmpty(thirdLevelReq))
                {
                    throw new InvalidUrlException(@"Category required for ""search"" request");
                }
                else
                {
                    return new SearchRequestItem(id, title, thirdLevelReq, parameterString);
                }
            }
            return null;
        }
    }
}
