using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.Interfaces;
using MetacriticScraper.MediaData;
using HtmlAgilityPack;

namespace MetacriticScraper.RequestData
{
    public class MovieRequestItem : RequestItem
    {
        public MovieRequestItem(string url)
        {
            MediaType = Constants.MovieTypeId;
        }

        public override List<string> Scrape()
        {
            List<string> urls = new List<string>();
            foreach (string url in Urls)
            {
                var task = m_webUtils.HttpGet(Constants.MetacriticURL + "/" + url, Constants.MetacriticURL, 30000);
                urls.Add(task.Result);
            }

            return urls;
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

                    short criticRating = -1;
                    criticRating = Int16.Parse(ParseItem(ref infoString, @"""ratingValue"" : """, @""""));
                    short criticRatingCount = -1;
                    criticRatingCount = Int16.Parse(ParseItem(ref infoString, @"""ratingCount"" : """, @""""));
                    movie.Rating = new Rating(criticRating, criticRatingCount);

                    infoString = infoString.Substring(infoString.IndexOf(@"""director"""));
                    movie.Director = new Person(ParseItem(ref infoString, @"""name"": """, @""""));

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
    }
}
