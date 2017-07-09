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
        public AlbumRequestItem(string id, string title, string thirdLevelRequest) :
            base(id, title, thirdLevelRequest)
        {
            MediaType = Constants.AlbumTypeId;
        }

        public AlbumRequestItem(string id, string title, string releaseYear, string thirdLevelRequest) :
            base(id, title, releaseYear, thirdLevelRequest)
        {
            MediaType = Constants.AlbumTypeId;
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
            Logger.Info("{0} urls filtered for {1}", Urls.Count, SearchString);
            return Urls.Count > 0;
        }


        public override MediaItem Parse(string html)
        {
            Album album = new Album();
            album.Title = ParseItem(ref html, @"<span itemprop=""name"">", @"</span>");
            album.PrimaryArtist = ParseItem(ref html, @"<span class=""band_name"" itemprop=""name"">", @"</span>");
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

        public override bool Equals(IResult obj)
        {
            bool result = false;
            if (base.Equals(obj))
            {
                string name = obj.Name.Split('-')[0].Trim();
                result = string.Equals(Name, name, StringComparison.OrdinalIgnoreCase);
                if (result && ItemDate != null)
                {
                    result = string.Equals(ItemDate, obj.ItemDate);
                }
            }
            return result;
        }
    }
}
