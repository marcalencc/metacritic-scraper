using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;
using MetacriticScraper.MediaData;

namespace MetacriticScraper.RequestData
{
    public class MovieRequestItem : RequestItem
    {
        public MovieRequestItem(string id, string title) : base(id, title)
        {
            MediaType = Constants.MovieTypeId;
        }

        public MovieRequestItem(string id, string title, string releaseYear) : base(id, title, releaseYear)
        {
            MediaType = Constants.MovieTypeId;
        }

        public override List<string> Scrape()
        {
            Logger.Info("Scraping {0} urls for {1}", Urls.Count, SearchString);
            List<string> htmlResponses = new List<string>();
            foreach (string url in Urls)
            {
                var task = m_webUtils.HttpGet(Constants.MetacriticURL + "/" + url, Constants.MetacriticURL, 30000);
                htmlResponses.Add(task.Result);
            }

            return htmlResponses;
        }

        public override bool FilterValidUrls()
        {
            Urls = m_autoResult.Where(r => this.Equals(r)).Select(r => r.Url).ToList();
            Logger.Info("{0} urls filtered for {1}", Urls.Count, SearchString);
            return Urls.Count > 0;
        }

        public override MediaItem Parse(string html)
        {
            int startIndex = html.IndexOf(@"<script type=""application/ld+json"">");
            if (startIndex != -1)
            {
                string infoString = html.Substring(startIndex);
                int endIndex = infoString.IndexOf(@"</script>");
                if (endIndex != -1)
                {
                    Movie movie = new Movie();
                    infoString = infoString.Substring(0, endIndex);

                    movie.Title = ParseItem(ref infoString, @"""name"" : """, @"""");

                    string releaseDateStr = ParseItem(ref infoString, @"""datePublished"" : """, @"""");
                    DateTime releaseDate;
                    if (DateTime.TryParse(releaseDateStr, out releaseDate))
                    {
                        movie.ReleaseDate = releaseDate;
                    }

                    short criticRating = 0;
                    short criticRatingCount = 0;
                    if (short.TryParse(ParseItem(ref infoString, @"""ratingValue"" : """, @""""), out criticRating))
                    {
                        criticRatingCount = Int16.Parse(ParseItem(ref infoString, @"""ratingCount"" : """, @""""));
                    }
                    movie.Rating = new Rating(criticRating, criticRatingCount);

                    infoString = infoString.Substring(infoString.IndexOf(@"""director"""));
                    movie.Director = ParseItem(ref infoString, @"""name"": """, @"""");

                    return movie;
                }
            }

            return null;
        }

        private string ParseItem(ref string infoStr, string startPos, string endPos)
        {
            int startIndex = infoStr.IndexOf(startPos) + startPos.Length;
            infoStr = infoStr.Substring(startIndex);
            int endIndex = infoStr.IndexOf(endPos);
            return infoStr.Substring(0, endIndex);
        }

        public override bool Equals(IResult obj)
        {
            bool result = false;
            if (base.Equals(obj))
            {
                result = string.Equals(Name, obj.Name, StringComparison.OrdinalIgnoreCase);
                if (result && ItemDate != null)
                {
                    result = string.Equals(ItemDate, obj.ItemDate);
                }
            }
            return result;
        }
    }
}
