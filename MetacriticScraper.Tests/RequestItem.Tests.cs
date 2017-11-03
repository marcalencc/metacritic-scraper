using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using MetacriticScraper.Interfaces;
using MetacriticScraper.RequestData;
using MetacriticScraper.JSONObjects;
using MetacriticScraper.MediaData;
using MetacriticScraper.Scraper;
using Moq;
using Newtonsoft.Json;

namespace MetacriticScraper.Tests
{
    [TestFixture]
    public class RequestItemTest
    {
        [OneTimeSetUp]
        public void Setup()
        {
        }

        [OneTimeTearDown]
        public void TearDown()
        {
        }


        [Test]
        public async Task Test_RequestItem_AutoSearch()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_the_master_autosearch.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.Setup(p => p.HttpPost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData));

            MovieRequestItem item = new MovieRequestItem("1", "the master", "");
            item.WebUtils = webUtils.Object;
            bool result = await item.AutoSearch();

            Assert.IsTrue(result);
        }

        [Test]
        public void Test_RequestItem_MovieFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_the_master_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            MovieRequestItem item = new MovieRequestItem("1", "the master", "");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_RequestItem_MovieFilterValidUrlsWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_the_master_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            MovieRequestItem item = new MovieRequestItem("1", "the master", "", "details");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/movie/the-master/details");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_RequestItem_MovieScrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2005 = File.ReadAllText(dir + @"\TestData\movie_moonlight_2005_scraped.txt");
            string testData_2016 = File.ReadAllText(dir + @"\TestData\movie_moonlight_2016_scraped.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.SetupSequence(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData_2005)).
                Returns(Task.FromResult(testData_2016));

            MovieRequestItem item = new MovieRequestItem("1", "moonlight", "");
            item.Urls = new List<string>();
            item.Urls.Add(@"/movie/moonlight");
            item.Urls.Add(@"/movie/moonlight-2016");
            item.WebUtils = webUtils.Object;
            List <UrlResponsePair> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 2);
        }

        [Test]
        public void Test_RequestItem_MovieParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\movie_moonlight_2016_scraped.txt");

            MovieRequestItem item = new MovieRequestItem("1", "moonlight", "");
            item.UrlImagePath = new Dictionary<string, string>();
            item.UrlImagePath.Add(@"\movie\moonlight", "tempmovie.jpg");
            IMetacriticData mItem = item.Parse(new UrlResponsePair(@"\movie\moonlight", testData_2016));

            Assert.AreEqual(((Movie) mItem).Title, "Moonlight");
            Assert.AreEqual(((Movie) mItem).Director, "Barry Jenkins");
            Assert.AreEqual(((Movie) mItem).Rating.CriticRating, 99);
            Assert.AreEqual(((Movie) mItem).Rating.CriticReviewCount, 51);
            Assert.AreEqual(((Movie) mItem).Rating.UserRating, 7.2f);
            Assert.AreEqual(((Movie) mItem).Rating.UserReviewCount, 994);
            Assert.AreEqual(((Movie) mItem).ImageUrl, "tempmovie.jpg");
        }

        [Test]
        public void Test_RequestItem_MovieParse2()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_her_scraped.txt");

            MovieRequestItem item = new MovieRequestItem("1", "moonlight", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((Movie) mItem).Title, "Her");
            Assert.AreEqual(((Movie) mItem).Director, "Spike Jonze");
            Assert.AreEqual(((Movie) mItem).Rating.CriticRating, 90);
            Assert.AreEqual(((Movie) mItem).Rating.CriticReviewCount, 46);
            Assert.AreEqual(((Movie) mItem).Rating.UserRating, 8.6f);
            Assert.AreEqual(((Movie) mItem).Rating.UserReviewCount, 1376);
        }


        [Test]
        public void Test_RequestItem_MovieParse3()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_thewolfofwallstreet_scraped.txt");

            MovieRequestItem item = new MovieRequestItem("1", "the wolf of wall street", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((Movie) mItem).Title, "The Wolf of Wall Street");
            Assert.AreEqual(((Movie) mItem).Director, "Martin Scorsese");
            Assert.AreEqual(((Movie) mItem).Rating.CriticRating, 75);
            Assert.AreEqual(((Movie) mItem).Rating.CriticReviewCount, 47);
            Assert.AreEqual(((Movie) mItem).Rating.UserRating, 6.8f);
            Assert.AreEqual(((Movie) mItem).Rating.UserReviewCount, 1856);
        }

        [Test]
        public void Test_RequestItem_MovieParseWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_zerodarkthirty_details.txt");

            MovieRequestItem item = new MovieRequestItem("1", "zero dark thirty", "", "details");
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((MediaDetail) mItem).Details.Count, 8);
            CollectionAssert.Contains(((MediaDetail) mItem).Details,
                new DetailItem("Runtime", "157 min"));
            CollectionAssert.Contains(((MediaDetail) mItem).Credits,
                new MediaCredit("Jessica Chastain", "Maya"));
            CollectionAssert.Contains(((MediaDetail) mItem).Credits,
                new MediaCredit("Nabil Elouahabi", "Detainee On Monitor"));
            CollectionAssert.Contains(((MediaDetail) mItem).Credits,
                new MediaCredit("Jonathan Leven", "Co-Producer"));
        }

        // Album

        [Test]
        public void Test_RequestItem_AlbumFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_lemonade_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/music/lemonade/beyonce");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_RequestItem_AlbumScrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_lemonade_2016_scraped.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.Setup(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData));

            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            item.Urls = new List<string>();
            item.Urls.Add(@"/music/lemonade/beyonce");
            item.WebUtils = webUtils.Object;
            List<UrlResponsePair> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 1);
        }

        [Test]
        [Ignore("Obsolete due to Metacritic site update")]
        public void Test_RequestItem_AlbumParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_lemonade_2016_scraped.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            item.UrlImagePath = new Dictionary<string, string>();
            item.UrlImagePath.Add(@"\album\lemonade", "tempalbum.jpg");
            IMetacriticData mItem = item.Parse(new UrlResponsePair(@"\album\lemonade", testData));

            Assert.AreEqual(((Album) mItem).Title, "Lemonade");
            Assert.AreEqual(((Album) mItem).Rating.CriticRating, 92);
            Assert.AreEqual(((Album) mItem).Rating.CriticReviewCount, 33);
            Assert.AreEqual(((Album) mItem).Rating.UserRating, 7.7f);
            Assert.AreEqual(((Album) mItem).Rating.UserReviewCount, 2518);
            Assert.AreEqual(((Album) mItem).ImageUrl, "tempalbum.jpg");
        }

        [Test]
        [Ignore("Obsolete due to Metacritic site update")]
        public void Test_RequestItem_AlbumParse2()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_melodrama_scraped.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "melodrama", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((Album) mItem).Title, "Melodrama");
            Assert.AreEqual(((Album) mItem).PrimaryArtist, "Lorde");
            Assert.AreEqual(((Album) mItem).Rating.CriticRating, 91);
            Assert.AreEqual(((Album) mItem).Rating.CriticReviewCount, 33);
            Assert.AreEqual(((Album) mItem).Rating.UserRating, 9.0f);
            Assert.AreEqual(((Album) mItem).Rating.UserReviewCount, 1363);
        }

        [Test]
        [Ignore("Obsolete due to Metacritic site update")]
        public void Test_RequestItem_AlbumParse3()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_aseatatthetable_scraped.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "a seat at the table", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((Album) mItem).Title, "A Seat at the Table");
            Assert.AreEqual(((Album) mItem).PrimaryArtist, "Solange");
            Assert.AreEqual(((Album) mItem).Rating.CriticRating, 89);
            Assert.AreEqual(((Album) mItem).Rating.CriticReviewCount, 26);
            Assert.AreEqual(((Album) mItem).Rating.UserRating, 8.2f);
            Assert.AreEqual(((Album) mItem).Rating.UserReviewCount, 301);
        }

        [Test]
        public void Test_RequestItem_AlbumParse4()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData_20171103\album_all_american_made.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "all american made", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((Album)mItem).Title, "All American Made");
            Assert.AreEqual(((Album)mItem).PrimaryArtist, "Margo Price");
            Assert.AreEqual(((Album)mItem).Rating.CriticRating, 82);
            Assert.AreEqual(((Album)mItem).Rating.CriticReviewCount, 13);
            Assert.AreEqual(((Album)mItem).Rating.UserRating, 6.3f);
            Assert.AreEqual(((Album)mItem).Rating.UserReviewCount, 4);
        }

        [Test]
        public void Test_RequestItem_AlbumParse5()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData_20171103\album_midnite_vultures.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "Midnite Vultures", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((Album)mItem).Title, "Midnite Vultures");
            Assert.AreEqual(((Album)mItem).PrimaryArtist, "Beck");
            Assert.AreEqual(((Album)mItem).Rating.CriticRating, 83);
            Assert.AreEqual(((Album)mItem).Rating.CriticReviewCount, 19);
            Assert.AreEqual(((Album)mItem).Rating.UserRating, 8.8f);
            Assert.AreEqual(((Album)mItem).Rating.UserReviewCount, 53);
        }

        [Test]
        public void Test_RequestItem_AlbumParseWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_teensofdenial_details.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "teens of denial", "", "details");
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((MediaDetail) mItem).Details.Count, 4);
            CollectionAssert.Contains(((MediaDetail) mItem).Details,
                new DetailItem("Record Label", "Matador"));
            CollectionAssert.Contains(((MediaDetail) mItem).Details,
                new DetailItem("Genre(s)", "Pop/Rock, Alternative/Indie Rock"));
            CollectionAssert.Contains(((MediaDetail) mItem).Details,
                new DetailItem("Name", "Car Seat Headrest"));
        }

        // TV Show

        [Test]
        public void Test_RequestItem_TvShowFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_RequestItem_TvShowFilterValidUrlsWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "", "details");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/tv/veep/details");
            Assert.IsTrue(result);
        }


        [Test]
        public void Test_RequestItem_TvShowFilterValidUrlsWithSeasonAndDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "details");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/tv/veep/season-6/details");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_RequestItem_TvShowScrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep_6_scraped.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.Setup(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData));

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6");
            item.Urls = new List<string>();
            item.Urls.Add(@"/tv/veep/season-6");
            item.WebUtils = webUtils.Object;
            List<UrlResponsePair> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 1);
        }

        [Test]
        [Ignore("Obsolete due to Metacritic site update")] 
        public void Test_RequestItem_TvShowParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep_6_scraped.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "");
            item.UrlImagePath = new Dictionary<string, string>();
            item.UrlImagePath.Add(@"\tv\veep", "temptv.jpg");
            IMetacriticData mItem = item.Parse(new UrlResponsePair(@"\tv\veep", testData));

            Assert.AreEqual(((TVShow) mItem).Title, "Veep");
            Assert.AreEqual(((TVShow) mItem).Rating.CriticRating, 88);
            Assert.AreEqual(((TVShow) mItem).Rating.CriticReviewCount, 15);
            Assert.AreEqual(((TVShow) mItem).Rating.UserRating, 8.1f);
            Assert.AreEqual(((TVShow) mItem).Rating.UserReviewCount, 36);
            Assert.AreEqual(((TVShow) mItem).ImageUrl, "temptv.jpg");
        }

        [Test]
        [Ignore("Obsolete due to Metacritic site update")]
        public void Test_RequestItem_TvShowParse2()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_curbyourenthusiasm_7_scraped.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "curb your enthusiasm", "7", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((TVShow) mItem).Title, "Curb Your Enthusiasm");
            Assert.AreEqual(((TVShow) mItem).Rating.CriticRating, 81);
            Assert.AreEqual(((TVShow) mItem).Rating.CriticReviewCount, 18);
            Assert.AreEqual(((TVShow) mItem).Rating.UserRating, 8.5f);
            Assert.AreEqual(((TVShow) mItem).Rating.UserReviewCount, 58);
        }

        [Test]
        [Ignore("Obsolete due to Metacritic site update")]
        public void Test_RequestItem_TvShowParse3()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_arrested_development_scraped.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "arrested development", "");
            item.UrlImagePath = new Dictionary<string, string>();
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((TVShow) mItem).Title, "Arrested Development");
            Assert.AreEqual(((TVShow) mItem).Rating.CriticRating, 89);
            Assert.AreEqual(((TVShow) mItem).Rating.CriticReviewCount, 26);
            Assert.AreEqual(((TVShow) mItem).Rating.UserRating, 9.2f);
            Assert.AreEqual(((TVShow) mItem).Rating.UserReviewCount, 426);
        }

        [Test]
        public void Test_RequestItem_TvShowParse4()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData_20171103\tv_six_feet_under_1.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "six-feet-under", "", "");
            item.UrlImagePath = new Dictionary<string, string>();
            item.UrlImagePath.Add(@"\tv\six-feet-under", "temptv.jpg");
            IMetacriticData mItem = item.Parse(new UrlResponsePair(@"\tv\six-feet-under", testData));

            Assert.AreEqual(((TVShow)mItem).Title, "Six Feet Under");
            Assert.AreEqual(((TVShow)mItem).Rating.CriticRating, 74);
            Assert.AreEqual(((TVShow)mItem).Rating.CriticReviewCount, 23);
            Assert.AreEqual(((TVShow)mItem).Rating.UserRating, 8.3f);
            Assert.AreEqual(((TVShow)mItem).Rating.UserReviewCount, 178);
            Assert.AreEqual(((TVShow)mItem).ImageUrl, "temptv.jpg");
        }

        [Test]
        public void Test_RequestItem_TvShowParse5()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData_20171103\tv_the_wire_2.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "the-wire", "2", "");
            item.UrlImagePath = new Dictionary<string, string>();
            item.UrlImagePath.Add(@"\tv\the-wire", "temptv.jpg");
            IMetacriticData mItem = item.Parse(new UrlResponsePair(@"\tv\the-wire", testData));

            Assert.AreEqual(((TVShow)mItem).Title, "The Wire");
            Assert.AreEqual(((TVShow)mItem).Season, 2);
            Assert.AreEqual(((TVShow)mItem).Rating.CriticRating, 95);
            Assert.AreEqual(((TVShow)mItem).Rating.CriticReviewCount, 17);
            Assert.AreEqual(((TVShow)mItem).Rating.UserRating, 9.2f);
            Assert.AreEqual(((TVShow)mItem).Rating.UserReviewCount, 344);
            Assert.AreEqual(((TVShow)mItem).ImageUrl, "temptv.jpg");
        }

        [Test]
        public void Test_RequestItem_TvShowParseWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep_details.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "details");
            IMetacriticData mItem = item.Parse(new UrlResponsePair("url", testData));

            Assert.AreEqual(((MediaDetail) mItem).Details.Count, 4);
            CollectionAssert.Contains(((MediaDetail) mItem).Credits,
                new MediaCredit("Julia Louis-Dreyfus", "Vice President Selina Meyer"));
            CollectionAssert.Contains(((MediaDetail) mItem).Details,
                new DetailItem("Creators", "Armando Iannucci"));
        }

        [Test]
        public void Test_RequestItem_RetrieveImagePath()
        {
            MovieRequestItem item = new MovieRequestItem("1", "moonlight", "");
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_moonlight_autosearch.txt");

            item.AutoResult = JsonConvert.DeserializeObject<RootObject>(testData).AutoComplete.Results;
            item.FilterValidUrls();
            item.RetrieveImagePath();

            Assert.AreEqual(item.UrlImagePath["/movie/moonlight"],
                @"http://static.metacritic.com/images/products/movies/5/f297a665fd50e664244005952493fbd0-98.jpg");
        }

        [Test]
        public void Test_RequestItem_RetrieveImagePath2()
        {
            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_lemonade_autosearch.txt");

            item.AutoResult = JsonConvert.DeserializeObject<RootObject>(testData).AutoComplete.Results;
            item.FilterValidUrls();
            item.RetrieveImagePath();

            Assert.AreEqual(item.UrlImagePath["/music/lemonade/beyonce"],
                @"http://static.metacritic.com/images/products/music/9/db45c5f14e2271eda363d1dcc264f384-98.jpg");
        }

        [Test]
        public void Test_RequestItem_RetrieveImagePath3()
        {
            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "");
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep_autosearch.txt");

            item.AutoResult = JsonConvert.DeserializeObject<RootObject>(testData).AutoComplete.Results;
            item.FilterValidUrls();
            item.RetrieveImagePath();

            Assert.AreEqual(item.UrlImagePath["/tv/veep"],
                @"http://static.metacritic.com/images/products/tv/6/760dff15565dd81e02be67f0c9d29730-98.jpg");
        }


    }
}
