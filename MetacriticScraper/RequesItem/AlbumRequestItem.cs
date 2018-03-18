using System;
using System.Collections.Generic;
using System.Linq;
using MetacriticScraper.Scraper;
using MetacriticScraper.MediaData;
using MetacriticScraper.Interfaces;

namespace MetacriticScraper.RequestData
{
    public class AlbumRequestItem : RequestItem
    {
        public AlbumRequestItem(string id, string title, string thirdLevelRequest) :
            base(id, title, thirdLevelRequest)
        {
            m_websiteString = "/music/" + title;
            MediaType = Constants.AlbumTypeId;
        }

        public AlbumRequestItem(string id, string title, string releaseYear, string thirdLevelRequest) :
            base(id, title, releaseYear, thirdLevelRequest)
        {
            MediaType = Constants.AlbumTypeId;
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
                Album album = new Album();
                album.Title = ParseItem(ref html, @"<h1>", @"</h1>");
                album.PrimaryArtist = ParseItem(ref html, @"<span class=""band_name"" itemprop=""name"">", @"</span>");
                string releaseDateStr = ParseItem(ref html, @"<span class=""data"" itemprop=""datePublished"">", @"</span>");
                DateTime releaseDate;
                if (DateTime.TryParse(releaseDateStr, out releaseDate))
                {
                    album.ReleaseDate = releaseDate.ToString("MM/dd/yyyy");
                }

                short? criticRating = null;
                short? criticRatingCount = null;
                if (short.TryParse(ParseItem(ref html, @"<span itemprop=""ratingValue"">", @"</span>"), out short tempCriticRating))
                {
                    criticRating = tempCriticRating;
                    criticRatingCount = Int16.Parse(ParseItem(ref html, @"<span itemprop=""reviewCount"">", @"</span>"));
                }

                float? userRating = null;
                short? userRatingCount = null;
                int userRatingIdx = html.IndexOf("metascore_w user large");
                if (userRatingIdx != -1)
                {
                    html = html.Substring(userRatingIdx);
                    if (float.TryParse(ParseItem(ref html, @""">", @"</div>"), out float tempUserRating))
                    {
                        userRating = tempUserRating;
                        userRatingCount = Int16.Parse(ParseItem(ref html, @"user-reviews"">", @" Ratings"));
                    }
                }

                album.Rating = new Rating(criticRating, userRating, criticRatingCount, userRatingCount);

                if (UrlImagePath != null)
                {
                    string imgPath;
                    if (UrlImagePath.TryGetValue(urlResponsePair.Url, out imgPath))
                    {
                        album.ImageUrl = imgPath;
                    }
                }

                return album;
            }
            else if (m_thirdLevelRequest == "details")
            {
                MediaDetail mediaDetail = new MediaDetail();
                html = html.Substring(html.IndexOf(@"""new_details"""));
                while (html.Contains(@"span class=""label"">"))
                {
                    string desc = ParseItem(ref html, @"span class=""label"">", @":</span>");
                    string value = string.Empty;

                    int nextIdx;
                    int valueIdx;
                    do
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            value = ParseItem(ref html, @"span class=""data"">", @"</span>");
                        }
                        else
                        {
                            value = string.Format("{0}, {1}", value,
                                ParseItem(ref html, @"span class=""data"">", @"</span>"));
                        }

                        nextIdx = html.IndexOf(@"<span class=""label"">");
                        valueIdx = html.IndexOf(@"span class=""data"">");
                    } while (nextIdx != -1 && valueIdx < nextIdx);

                    DetailItem detail = new DetailItem(desc, value);
                    mediaDetail.Details.Add(detail);
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
                string name = obj.Name.Split(new string[] { " - " }, StringSplitOptions.None)[0].Trim();
                result = string.Equals(SimplifyRequestName(Name), SimplifyRequestName(name),
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
