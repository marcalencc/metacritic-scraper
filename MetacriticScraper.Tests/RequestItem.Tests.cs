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
        public async Task Test_RequestItem_AutoSearch()
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
        public void Test_RequestItem_MovieFilterValidUrls()
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
        public void Test_RequestItem_MovieFilterValidUrlsWithDetails()
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
        public void Test_RequestItem_MovieScrape()
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
        public void Test_RequestItem_MovieParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\moonlight_2016_scraped.txt");

            MovieRequestItem item = new MovieRequestItem("1", "moonlight", "");
            MetacriticData mItem = item.Parse(testData_2016);

            Assert.AreEqual(((Movie) mItem).Title, "Moonlight");
            Assert.AreEqual(((Movie) mItem).Rating.CriticRating, 99);
            Assert.AreEqual(((Movie) mItem).Rating.CriticReviewCount, 51);
        }

        [Test]
        public void Test_RequestItem_MovieParseWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\movie_zerodarkthirty_details.txt");

            MovieRequestItem item = new MovieRequestItem("1", "zero dark thirty", "", "details");
            MetacriticData mItem = item.Parse(testData_2016);

            Assert.AreEqual(((Movie) mItem).Title, null);
            Assert.AreEqual(((Movie) mItem).Details.Count, 60);
            CollectionAssert.Contains(((Movie) mItem).Details, new Detail("Runtime",
                "157 min"));
            CollectionAssert.Contains(((Movie) mItem).Details, new Detail("Jessica Chastain",
                "Maya"));
            CollectionAssert.Contains(((Movie) mItem).Details, new Detail("Nabil Elouahabi",
                "Detainee On Monitor"));
            CollectionAssert.Contains(((Movie) mItem).Details, new Detail("Jonathan Leven",
                "Co-Producer"));
        }

        // Album

        [Test]
        public void Test_RequestItem_AlbumFilterValidUrls()
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
        public void Test_RequestItem_AlbumScrape()
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
        public void Test_RequestItem_AlbumParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\lemonade_2016_scraped.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "lemonade", "");
            MetacriticData mItem = item.Parse(testData_2016);

            Assert.AreEqual(((Album) mItem).Title, "Lemonade");
            Assert.AreEqual(((Album) mItem).Rating.CriticRating, 92);
            Assert.AreEqual(((Album) mItem).Rating.CriticReviewCount, 33);
        }

        [Test]
        public void Test_RequestItem_AlbumParseWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\album_teensofdenial_details.txt");

            AlbumRequestItem item = new AlbumRequestItem("1", "teens of denial", "", "details");
            MetacriticData mItem = item.Parse(testData_2016);

            Assert.AreEqual(((Album) mItem).Title, null);
            Assert.AreEqual(((Album) mItem).Details.Count, 4);
            CollectionAssert.Contains(((Album) mItem).Details, new Detail("Record Label", "Matador"));
            CollectionAssert.Contains(((Album) mItem).Details, new Detail("Genre(s)",
                "Pop/Rock, Alternative/Indie Rock"));
            CollectionAssert.Contains(((Album) mItem).Details, new Detail("Name",
                "Car Seat Headrest"));
        }

        // TV Show

        [Test]
        public void Test_RequestItem_TvShowFilterValidUrls()
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
        public void Test_RequestItem_TvShowFilterValidUrlsWithDetails()
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
        public void Test_RequestItem_TvShowFilterValidUrlsWithSeasonAndDetails()
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
        public void Test_RequestItem_TvShowScrape()
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
        public void Test_RequestItem_TvShowParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\veep_6_scraped.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "");
            MetacriticData mItem = item.Parse(testData_2016);

            Assert.AreEqual(((TVShow) mItem).Title, "Veep");
            Assert.AreEqual(((TVShow) mItem).Rating.CriticRating, 88);
            Assert.AreEqual(((TVShow) mItem).Rating.CriticReviewCount, 15);
        }


        [Test]
        public void Test_RequestItem_TvShowParseWithDetails()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData_2016 = File.ReadAllText(dir + @"\TestData\tvshow_veep_details.txt");

            TVShowRequestItem item = new TVShowRequestItem("1", "veep", "6", "details");
            MetacriticData mItem = item.Parse(testData_2016);

            Assert.AreEqual(((TVShow)mItem).Title, null);
            Assert.AreEqual(((TVShow)mItem).Details.Count, 12);
            CollectionAssert.Contains(((TVShow)mItem).Details, new Detail("Julia Louis-Dreyfus",
                "Vice President Selina Meyer"));
            CollectionAssert.Contains(((TVShow)mItem).Details, new Detail("Creators",
                "Armando Iannucci"));
        }

    }
}
