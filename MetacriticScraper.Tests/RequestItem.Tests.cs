using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using NUnit.Framework;
using MetacriticScraper.Scraper;
using MetacriticScraper.Errors;
using MetacriticScraper.Interfaces;
using MetacriticScraper.RequestData;
using MetacriticScraper.JSONObjects;
using MetacriticScraper.MediaData;
using Moq;

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
        public async Task TestAutoSearch()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_the_master.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.Setup(p => p.HttpPost(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData));

            MovieRequestItem item = new MovieRequestItem("1", "the master", "");
            item.WebUtils = webUtils.Object;
            bool result = await item.AutoSearch();

            Assert.IsTrue(result);
        }

        [Test]
        public void TestMovieFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_the_master.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            MovieRequestItem item = new MovieRequestItem("1", "the master", "");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestMovieFilterValidUrlsWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\movie_the_master.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            MovieRequestItem item = new MovieRequestItem("1", "the master", "", "details");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/movie/the-master/details");
            Assert.IsTrue(result);
        }

        [Test]
        public void TestMovieScrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2005 = File.ReadAllText(dir + @"\TestData\moonlight_2005_scraped.txt");
            string testData_2016 = File.ReadAllText(dir + @"\TestData\moonlight_2016_scraped.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.SetupSequence(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData_2005)).
                Returns(Task.FromResult(testData_2016));

            MovieRequestItem item = new MovieRequestItem("1", "moonlight", "");
            item.Urls = new List<string>();
            item.Urls.Add(@"/movie/moonlight");
            item.Urls.Add(@"/movie/moonlight-2016");
            item.WebUtils = webUtils.Object;
            List <string> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 2);
        }

        [Test]
        public void TestMovieParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\moonlight_2016_scraped.txt");

            MovieRequestItem item = new MovieRequestItem("1", "moonlight", "");
            MediaItem mItem = item.Parse(testData_2016);

            Assert.AreEqual(mItem.Title, "Moonlight");
            Assert.AreEqual(mItem.Rating.CriticRating, 99);
            Assert.AreEqual(mItem.Rating.CriticReviewCount, 51);
        }

        // Album

        [Test]
        public void TestAlbumFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\album_lemonade.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/music/lemonade/beyonce");
            Assert.IsTrue(result);
        }

        [Test]
        public void TestAlbumScrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\lemonade_2016_scraped.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.Setup(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData));

            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            item.Urls = new List<string>();
            item.Urls.Add(@"/music/lemonade/beyonce");
            item.WebUtils = webUtils.Object;
            List<string> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 1);
        }

        [Test]
        public void TestAlbumParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\lemonade_2016_scraped.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            MediaItem mItem = item.Parse(testData_2016);

            Assert.AreEqual(mItem.Title, "Lemonade");
            Assert.AreEqual(mItem.Rating.CriticRating, 92);
            Assert.AreEqual(mItem.Rating.CriticReviewCount, 33);
        }

        // TV Show

        [Test]
        public void TestTvShowFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.IsTrue(result);
        }

        [Test]
        public void TestTvShowFilterValidUrlsWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "", "details");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/tv/veep/details");
            Assert.IsTrue(result);
        }


        [Test]
        public void TestTvShowFilterValidUrlsWithSeasonAndDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\tvshow_veep.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "details");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/tv/veep/season-6/details");
            Assert.IsTrue(result);
        }

        [Test]
        public void TestTvShowScrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\veep_6_scraped.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.Setup(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData));

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6");
            item.Urls = new List<string>();
            item.Urls.Add(@"/tv/veep/season-6");
            item.WebUtils = webUtils.Object;
            List<string> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 1);
        }

        [Test]
        public void TestTvShowParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\veep_6_scraped.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "");
            MediaItem mItem = item.Parse(testData_2016);

            Assert.AreEqual(mItem.Title, "Veep");
            Assert.AreEqual(mItem.Rating.CriticRating, 88);
            Assert.AreEqual(mItem.Rating.CriticReviewCount, 15);
        }


        [Test]
        public void TestTvShowParseWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\tvshow_veep_details.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "details");
            MediaItem mItem = item.Parse(testData_2016);

            Assert.AreEqual(mItem.Title, null);
            Assert.AreEqual(mItem.Details.Count, 12);
            CollectionAssert.Contains(mItem.Details, new Detail("Julia Louis-Dreyfus",
                "Vice President Selina Meyer"));
            CollectionAssert.Contains(mItem.Details, new Detail("Creators",
                "Armando Iannucci"));
        }

    }
}
