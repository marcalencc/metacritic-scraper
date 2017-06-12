using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetacriticScraper.MediaData;

namespace MetacriticScraper.RequestData
{
    public class TVShowRequestItem : RequestItem
    {
        private string m_season;

        public TVShowRequestItem(string id, string title) : base(id, title)
        {
            MediaType = Constants.TvShowTypeId;
        }

        public TVShowRequestItem(string id, string title, string season) : this(id, title)
        {
            m_season = season;
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
            Urls = m_autoResult.Where(r => this.Equals(r)).Select(r => r.Url + "/season-" + m_season).ToList();
            return Urls.Count > 0;
        }

        public override MediaItem Parse(string html)
        {
            TVShow tvShow = new TVShow();
            tvShow.Title = ParseItem(ref html, @"<span itemprop=""name"">", @"</span>");
            tvShow.Season = Int32.Parse(ParseItem(ref html, @"Season ", @"</span>"));

            tvShow.Studio = ParseItem(ref html, @"<span itemprop=""name"">", @"</span>");

            short criticRating = 0;
            short criticRatingCount = 0;
            if (short.TryParse(ParseItem(ref html, @"<span itemprop=""ratingValue"">", @"</span>"), out criticRating))
            {
                criticRatingCount = Int16.Parse(ParseItem(ref html, @"<span itemprop=""reviewCount"">", @"</span>"));
            }
            tvShow.Rating = new Rating(criticRating, criticRatingCount);

            string releaseDateStr = ParseItem(ref html, @"<span class=""data"" itemprop=""startDate"">", @"</span>");
            DateTime releaseDate;
            if (DateTime.TryParse(releaseDateStr, out releaseDate))
            {
                tvShow.ReleaseDate = releaseDate;
            }
            return tvShow;
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
