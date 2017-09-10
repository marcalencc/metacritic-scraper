using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MetacriticScraper.Scraper;
using MetacriticScraper.Interfaces;
using MetacriticScraper.RequestData;
using Moq;

namespace MetacriticScraper.Tests
{
    [TestFixture]
    public class SearchRequestItemTest
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
        public void Test_FilterValidUrls_WithoutOffsetAndLimit()
        {
            string param = "sort=relevancy";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=0&sort=relevancy");
        }

        [Test]
        public void Test_FilterValidUrls_WithOffset()
        {
            string param = "sort=relevancy&offset=20";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 2);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=0&sort=relevancy");
            Assert.AreEqual(item.Urls[1], "search/movie/heart/results?page=1&sort=relevancy");
        }

        [Test]
        public void Test_FilterValidUrls_WithOffset2()
        {
            string param = "sort=relevancy&offset=59";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 2);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=2&sort=relevancy");
            Assert.AreEqual(item.Urls[1], "search/movie/heart/results?page=3&sort=relevancy");
        }

        [Test]
        public void Test_FilterValidUrls_WithLimit()
        {
            string param = "limit=20";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=0&sort=");
        }

        [Test]
        public void Test_FilterValidUrls_WithLimit2()
        {
            string param = "sort=score&limit=41";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "tvshow", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 3);
            Assert.AreEqual(item.Urls[0], "search/tv/heart/results?page=0&sort=score");
            Assert.AreEqual(item.Urls[1], "search/tv/heart/results?page=1&sort=score");
            Assert.AreEqual(item.Urls[2], "search/tv/heart/results?page=2&sort=score");
        }

        [Test]
        public void Test_FilterValidUrls_WithLimitAndOffset()
        {
            string param = "offset=21&limit=21";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 2);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=1&sort=");
            Assert.AreEqual(item.Urls[1], "search/movie/heart/results?page=2&sort=");
        }

        [Test]
        public void Test_FilterValidUrls_WithLimitAndOffset2()
        {
            string param = "offset=20&limit=56";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "tvshow", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 4);
            Assert.AreEqual(item.Urls[0], "search/tv/heart/results?page=0&sort=");
            Assert.AreEqual(item.Urls[1], "search/tv/heart/results?page=1&sort=");
            Assert.AreEqual(item.Urls[2], "search/tv/heart/results?page=2&sort=");
            Assert.AreEqual(item.Urls[3], "search/tv/heart/results?page=3&sort=");
        }

        [Test]
        public void Test_FilterValidUrls_WithLimitAndOffset3()
        {
            string param = "offset=61&limit=20";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "tvshow", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/tv/heart/results?page=3&sort=");
        }
    }
}
