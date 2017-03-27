using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.MediaData;

namespace MetacriticScraper.RequestData
{
    public class AlbumRequestItem : RequestItem
    {
        public AlbumRequestItem(string url)
        {
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

        public override bool FilterValidUrls()
        {
            Urls = m_autoResult.Where(r => r.Equals(this)).Select(r => r.Url).ToList();
            return Urls.Count > 0;
        }


        public override MediaItem Parse(string html)
        {
            Album album = new Album();
            album.Title = ParseItem(ref html, @"<span itemprop=""name"">", @"</span>");
            album.PrimaryArtist = new Person(ParseItem(ref html, @"<span class=""band_name"" itemprop=""name"">", @"</span>"));
            string releaseDateStr = ParseItem(ref html, @"<span class=""data"" itemprop=""datePublished"">", @"</span>");
            DateTime releaseDate;
            if (DateTime.TryParse(releaseDateStr, out releaseDate))
            {
                album.ReleaseDate = releaseDate;
            }

            short criticRating = 0;
            short criticRatingCount = 0;
            if (short.TryParse(ParseItem(ref html, @"<span itemprop=""ratingValue"">", @"</span>"), out criticRating))
            {
                criticRatingCount = Int16.Parse(ParseItem(ref html, @"<span itemprop=""reviewCount"">", @"</span>"));
            }

            album.Rating = new Rating(criticRating, criticRatingCount);
            return album;
        }

        private string ParseItem(ref string infoStr, string startPos, string endPos)
        {
            int startIndex = infoStr.IndexOf(startPos) + startPos.Length;
            infoStr = infoStr.Substring(startIndex);
            int endIndex = infoStr.IndexOf(endPos);
            return infoStr.Substring(0, endIndex).Trim();
        }
    }
}
