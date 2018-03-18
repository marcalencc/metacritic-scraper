using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MetacriticScraper.Scraper;
using MetacriticScraper.Interfaces;
using MetacriticScraper.MediaData;

namespace MetacriticScraper.RequestData
{
    public class MovieRequestItem : RequestItem
    {
        public MovieRequestItem(string id, string title, string thirdLevelReq) :
            base(id, title, thirdLevelReq)
        {
            m_websiteString = "/movie/" + title;
            MediaType = Constants.MovieTypeId;
        }

        public MovieRequestItem(string id, string title, string releaseYear, string thirdLevelReq) :
            base(id, title, releaseYear, thirdLevelReq)
        {
            MediaType = Constants.MovieTypeId;
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

        public override IMetacriticData Parse(UrlResponsePair urlResponsePair)
        {
            string html = urlResponsePair.Response;
            if (String.IsNullOrEmpty(m_thirdLevelRequest))
            {
                Movie movie = new Movie();
                int startIndex = html.IndexOf(@"<script type=""application/ld+json"">");
                if (startIndex != -1)
                {
                    string infoString = html.Substring(startIndex);
                    int endIndex = infoString.IndexOf(@"</script>");
                    if (endIndex != -1)
                    {
                        infoString = infoString.Substring(0, endIndex);
                        movie.Title = ParseItem(ref infoString, @"""name"" : """, @"""");

                        string releaseDateStr = ParseItem(ref infoString, @"""datePublished"" : """, @"""");
                        DateTime releaseDate;
                        if (DateTime.TryParse(releaseDateStr, out releaseDate))
                        {
                            movie.ReleaseDate = releaseDate.ToString("MM/dd/yyyy"); ;
                        }

                        short? criticRating = null;
                        short? criticRatingCount = null;
                        if (infoString.Contains(@"""ratingValue"" : """))
                        {
                            if (short.TryParse(ParseItem(ref infoString, @"""ratingValue"" : """, @""""), out short tempCriticRating))
                            {
                                criticRating = tempCriticRating;
                                criticRatingCount = Int16.Parse(ParseItem(ref infoString, @"""ratingCount"" : """, @""""));
                            }

                            // Critic
                            html = html.Substring(html.IndexOf("Critics</span>"));
                        }

                        // User
                        float? userRating = null;
                        short? userRatingCount = null;
                        if (html.Contains(@">based on "))
                        {
                            html = html.Substring(html.IndexOf(">based on "));

                            if (short.TryParse(ParseItem(ref html, @">based on ", " Ratings"), out short tempUserRatingCount))
                            {
                                userRatingCount = tempUserRatingCount;
                                html = html.Substring(html.IndexOf("metascore_w user"));
                                userRating = float.Parse(ParseItem(ref html, @">", @"</span>"));
                            }
                        }

                        movie.Rating = new Rating(criticRating, userRating, criticRatingCount, userRatingCount);


                        infoString = infoString.Substring(infoString.IndexOf(@"""director"""));
                        movie.Director = ParseItem(ref infoString, @"""name"": """, @"""");
                    }
                }

                if (UrlImagePath != null)
                {
                    string imgPath;
                    if (UrlImagePath.TryGetValue(urlResponsePair.Url, out imgPath))
                    {
                        movie.ImageUrl = imgPath;
                    }
                }

                return movie;
            }
            else if (m_thirdLevelRequest == "details")
            {
                MediaDetail mediaDetail = new MediaDetail();
                while (html.Contains(@"<td class=""label"">"))
                {
                    string desc = ParseItem(ref html, @"<td class=""label"">", @":</td>");
                    string value = ParseItem(ref html, @"<td class=""data"">", @"</td>");
                    if (value.Contains("</a>"))
                    {
                        value = ParseItem(ref value, @""">", @"</a>");
                    }

                    if (value.Contains("<span>"))
                    {
                        value = value.Replace("<span>", "").Replace("</span>", "");
                    }

                    if (desc == "Genres" || desc == "Languages" || desc == "Countries")
                    {
                        Regex rgx = new Regex("\\s+");
                        value = rgx.Replace(value, " ");
                    }

                    DetailItem detail = new DetailItem(desc, value);
                    mediaDetail.Details.Add(detail);
                }

                while (html.Contains(@"<td class=""person"">"))
                {
                    string name = ParseItem(ref html, @"<td class=""person"">", @"</td>");
                    if (name.Contains("</a>"))
                    {
                        name = ParseItem(ref name, @""">", @"</a>");
                    }

                    string role = ParseItem(ref html, @"<td class=""role"">", @"</td>");

                    MediaCredit credit = new MediaCredit(name, role);
                    mediaDetail.Credits.Add(credit);
                }

                return mediaDetail;
            }

            return null;
        }

        public override bool Equals(IResult obj)
        {
            bool result = false;
            if (base.Equals(obj))
            {
                result = string.Equals(SimplifyRequestName(Name), SimplifyRequestName(obj.Name),
                    StringComparison.OrdinalIgnoreCase);
                if (result && !String.IsNullOrEmpty(ItemDate))
                {
                    result = string.Equals(ItemDate, obj.ItemDate);
                }
            }
            return result;
        }
    }
}
