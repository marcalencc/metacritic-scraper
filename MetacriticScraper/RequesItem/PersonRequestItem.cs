using System;
using System.Collections.Generic;
using System.Linq;
using MetacriticScraper.Scraper;
using MetacriticScraper.MediaData;
using MetacriticScraper.Errors;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.RequestData
{
    public class PersonRequestItem : RequestItem
    {
        public PersonRequestItem(string id, string title, string thirdLevelRequest) :
            base(id, title, thirdLevelRequest)
        {
            m_websiteString = "/person/" + title;
            MediaType = Constants.PersonTypeId;
        }

        public override List<UrlResponsePair> Scrape()
        {
            Logger.Info("Scraping {0} urls for {1}", Urls.Count, SearchString);
            List<UrlResponsePair> responses = new List<UrlResponsePair>();
            foreach (string url in Urls)
            {
                var task = m_webUtils.HttpGet(url, Constants.MetacriticURL, 30000);
                responses.Add(new UrlResponsePair(url, task.Result));
            }

            return responses;
        }

        public override bool FilterValidUrls()
        {
            Urls = m_autoResult.Where(r => this.Equals(r)).Select(r => r.Url).Distinct().ToList();
            SetThirdLevelRequest();
            Logger.Info("{0} urls filtered for {1}", Urls.Count, SearchString);
            return Urls.Count > 0;
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
                throw new InvalidUrlException(@"Category required for ""person"" request");
            }

            param = param + "&sort_options=date&num_items=100";
            Urls = Urls.Select(u => u + "?" + param).ToList();
        }

        public override IMetacriticData Parse(UrlResponsePair urlResponsePair)
        {
            string html = urlResponsePair.Response;
            string name = ParseItem(ref html, @"""og:title"" content=""", @""">");
            Person person = new Person(name);

            if (IsMediaTypeAvailable(html))
            {
                html = html.Substring(html.IndexOf(@"class=""credits person_credits"""));
                List<Person.CreditMediaItemPair> pairs = new List<Person.CreditMediaItemPair>();
                while (html.Contains(@"class=""title brief_metascore"""))
                {
                    html = html.Substring(html.IndexOf(@"class=""title brief_metascore""") +
                        @"class=""title brief_metascore""".Length);

                    short? criticScore = null; 
                    if (short.TryParse(ParseItem(ref html, @""">", "</span>"), out short tempCriticScore))
                    {
                        criticScore = tempCriticScore;
                    }

                    string id = ParseItem(ref html, @"href=""", @""">");

                    // Album url has different format
                    string[] parts = id.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length > 0 && parts[0] == "music")
                    {
                        id = TrimAlbumUrl(id);
                    }
                    else if (parts.Length > 0 && parts[0] == "tv")
                    {
                        id = id.Replace("tv/", "tvshow/").Replace(@"/season-", @"/");
                    }

                    string title = ParseItem(ref html, @""">", "</a>");
                    DateTime releaseDate;

                    if (!DateTime.TryParse(ParseItem(ref html, @"class=""year"">", "</td>"),
                        out releaseDate))
                    {
                        releaseDate = DateTime.MinValue;
                    }

                    string credit = ParseItem(ref html, @"class=""role"">", "</td>");

                    html = html.Substring(html.IndexOf(@"class=""score"">") +
                        @"class=""score"">".Length);
                    float? userScore = null;
                    if (float.TryParse(ParseItem(ref html, @""">", "</span>"), out float tempUserScore))
                    {
                        userScore = tempUserScore;
                    }

                    Rating rating = new Rating(criticScore, userScore);
                    MediaItem item = new MediaItem()
                    {
                        Id = id,
                        Title = title,
                        ReleaseDate = releaseDate.ToString("MM/dd/yyyy"),
                        Rating = rating
                    };

                    pairs.Add(new Person.CreditMediaItemPair(credit, item));
                }

                person.CreditMediaPairItems = pairs;
            }

            return person;
        }

        private string TrimAlbumUrl(string url)
        {
            return url.Substring(0, url.LastIndexOf('/')).Replace("music/", "album/");
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
                result = string.Equals(SimplifyRequestName(Name), SimplifyRequestName(obj.Name),
                    StringComparison.OrdinalIgnoreCase);
            }
            return result;
        }
    }
}
