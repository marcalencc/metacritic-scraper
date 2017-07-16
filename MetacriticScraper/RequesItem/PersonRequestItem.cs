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

        public override MetacriticData Parse(string html)
        {
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

                    short criticScore; 
                    if (!short.TryParse(ParseItem(ref html, @""">", "</span>"), out criticScore))
                    {
                        criticScore = -2;
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
                    float userScore;
                    if (!float.TryParse(ParseItem(ref html, @""">", "</span>"), out userScore))
                    {
                        userScore = -2f;
                    }

                    Rating rating = new Rating(criticScore, userScore);
                    MediaItem item = new MediaItem()
                    {
                        Title = title,
                        ReleaseDate = releaseDate,
                        Rating = rating
                    };

                    pairs.Add(new Person.CreditMediaItemPair(credit, item));
                }

                person.CreditMediaPairItems = pairs;
            }

            return person;
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

        private string ParseItem(ref string infoStr, string startPos, string endPos)
        {
            int startIndex = infoStr.IndexOf(startPos) + startPos.Length;
            infoStr = infoStr.Substring(startIndex);
            int endIndex = infoStr.IndexOf(endPos);
            return infoStr.Substring(0, endIndex).Trim();
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
