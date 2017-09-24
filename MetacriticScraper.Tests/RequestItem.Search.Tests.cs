using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Framework;
using MetacriticScraper.Scraper;
using MetacriticScraper.Interfaces;
using MetacriticScraper.RequestData;
using MetacriticScraper.MediaData;
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
        public void Test_Search_FilterValidUrls_WithoutOffsetAndLimit()
        {
            string param = "sort=relevancy";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=0&sort=relevancy");
        }

        [Test]
        public void Test_Search_FilterValidUrls_WithOffset()
        {
            string param = "sort=relevancy&offset=20";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 2);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=0&sort=relevancy");
            Assert.AreEqual(item.Urls[1], "search/movie/heart/results?page=1&sort=relevancy");
        }

        [Test]
        public void Test_Search_FilterValidUrls_WithOffset2()
        {
            string param = "sort=relevancy&offset=59";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 2);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=2&sort=relevancy");
            Assert.AreEqual(item.Urls[1], "search/movie/heart/results?page=3&sort=relevancy");
        }

        [Test]
        public void Test_Search_FilterValidUrls_WithLimit()
        {
            string param = "limit=20";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=0&sort=");
        }

        [Test]
        public void Test_Search_FilterValidUrls_WithLimit2()
        {
            // limit max is 20
            string param = "sort=score&limit=41";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "tvshow", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/tv/heart/results?page=0&sort=score");
        }

        [Test]
        public void Test_Search_FilterValidUrls_WithLimitAndOffset()
        {
            // limit max is 20
            string param = "offset=21&limit=21";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "movie", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/movie/heart/results?page=1&sort=");
        }

        [Test]
        public void Test_Search_FilterValidUrls_WithLimitAndOffset2()
        {
            // limit max is 20
            string param = "offset=20&limit=56";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "tvshow", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 2);
            Assert.AreEqual(item.Urls[0], "search/tv/heart/results?page=0&sort=");
            Assert.AreEqual(item.Urls[1], "search/tv/heart/results?page=1&sort=");
        }

        [Test]
        public void Test_Search_FilterValidUrls_WithLimitAndOffset3()
        {
            string param = "offset=61&limit=20";
            SearchRequestItem item = new SearchRequestItem("1", "heart", "tvshow", param);
            item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "search/tv/heart/results?page=3&sort=");
        }

        [Test]
        public void Test_Search_Scrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData0 = File.ReadAllText(dir + @"\TestData\search_love_0.txt");
            string testData1 = File.ReadAllText(dir + @"\TestData\search_love_1.txt");
            string testData2 = File.ReadAllText(dir + @"\TestData\search_love_2.txt");
            string testData3 = File.ReadAllText(dir + @"\TestData\search_love_3.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.SetupSequence(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).
                Returns(Task.FromResult(testData0)).
                Returns(Task.FromResult(testData1));

            SearchRequestItem item = new SearchRequestItem("1", "love", "movie", "offset=11&limit=65");
            item.Urls = new List<string>();
            item.Urls.Add(@"/search/movie/love/results?page=0");
            item.Urls.Add(@"/search/movie/love/results?page=1");
            item.WebUtils = webUtils.Object;
            List<UrlResponsePair> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 2);
            Assert.AreEqual(resp[0].SearchItemCount, 10);
            Assert.AreEqual(resp[1].SearchItemCount, 10);
            Assert.AreEqual(resp[0].SequenceNo, 1);
            Assert.AreEqual(resp[1].SequenceNo, 2);
        }

        [Test]
        public void Test_Search_Parse_First_Page()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            UrlResponsePair pair = new UrlResponsePair(@"/search/movie/love/results?page=0",
                File.ReadAllText(dir + @"\TestData\search_love_0.txt"), 1, 10);

            SearchRequestItem item = new SearchRequestItem("1", "love", "movie", "offset=11&limit=65");
            item.Urls = new List<string>();
            item.Urls.Add(@"/search/movie/love/results?page=0");
            item.Urls.Add(@"/search/movie/love/results?page=1");
            item.Urls.Add(@"/search/movie/love/results?page=2");
            item.Urls.Add(@"/search/movie/love/results?page=3");
            IMetacriticData resp = item.Parse(pair);

            CollectionAssert.AllItemsAreInstancesOfType(((SearchData)resp).SearchItems,
                typeof(SearchData.SearchItem));
            Assert.AreEqual(((SearchData)resp).TotalResultCount, 238);
            Assert.AreEqual(((SearchData)resp).SearchItems.Count, 10);
            Assert.AreEqual(((SearchData)resp).SearchItems[0].Title, "After Love");
            Assert.AreEqual(((SearchData)resp).SearchItems[5].Id, "/movie/love-rosie");
            Assert.AreEqual(((SearchData)resp).SearchItems[8].ReleaseDate, "April 11, 2014");
            Assert.AreEqual(((SearchData)resp).SearchItems[9].Rating.CriticRating, 39);
        }

        [Test]
        public void Test_Search_Parse_Middle_Page()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            UrlResponsePair pair = new UrlResponsePair(@"/search/movie/love/results?page=1",
                File.ReadAllText(dir + @"\TestData\search_love_1.txt"), 2, 20);

            SearchRequestItem item = new SearchRequestItem("1", "love", "movie", "offset=11&limit=65");
            item.Urls = new List<string>();
            item.Urls.Add(@"/search/movie/love/results?page=0");
            item.Urls.Add(@"/search/movie/love/results?page=1");
            item.Urls.Add(@"/search/movie/love/results?page=2");
            item.Urls.Add(@"/search/movie/love/results?page=3");
            IMetacriticData resp = item.Parse(pair);

            CollectionAssert.AllItemsAreInstancesOfType(((SearchData)resp).SearchItems,
                typeof(SearchData.SearchItem));
            Assert.IsNull(((SearchData)resp).TotalResultCount);
            Assert.AreEqual(((SearchData)resp).SearchItems.Count, 20);
            Assert.AreEqual(((SearchData)resp).SearchItems[0].Title, "In the Mood for Love");
            Assert.AreEqual(((SearchData)resp).SearchItems[4].Id, "/movie/love-actually");
            Assert.AreEqual(((SearchData)resp).SearchItems[11].ReleaseDate, "May 27, 1964");
            Assert.AreEqual(((SearchData)resp).SearchItems[17].Rating.CriticRating, 55);
        }

        [Test]
        public void Test_Search_Parse_Middle_Page2()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            UrlResponsePair pair = new UrlResponsePair(@"/search/movie/love/results?page=2",
                File.ReadAllText(dir + @"\TestData\search_love_2.txt"), 3, 20);

            SearchRequestItem item = new SearchRequestItem("1", "love", "movie", "offset=11&limit=65");
            item.Urls = new List<string>();
            item.Urls.Add(@"/search/movie/love/results?page=0");
            item.Urls.Add(@"/search/movie/love/results?page=1");
            item.Urls.Add(@"/search/movie/love/results?page=2");
            item.Urls.Add(@"/search/movie/love/results?page=3");
            IMetacriticData resp = item.Parse(pair);

            CollectionAssert.AllItemsAreInstancesOfType(((SearchData)resp).SearchItems,
                typeof(SearchData.SearchItem));
            Assert.IsNull(((SearchData)resp).TotalResultCount);
            Assert.AreEqual(((SearchData)resp).SearchItems.Count, 20);
            Assert.AreEqual(((SearchData)resp).SearchItems[1].Title, "To Rome with Love");
            Assert.AreEqual(((SearchData)resp).SearchItems[2].Rating.CriticRating, 0);
            Assert.AreEqual(((SearchData)resp).SearchItems[9].ReleaseDate, "August 9, 2013");
            Assert.AreEqual(((SearchData)resp).SearchItems[17].Id, "/movie/love-sex");
        }

        [Test]
        public void Test_Search_Parse_Last_Page()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            UrlResponsePair pair = new UrlResponsePair(@"/search/movie/love/results?page=3",
                File.ReadAllText(dir + @"\TestData\search_love_3.txt"), 4, 15);

            SearchRequestItem item = new SearchRequestItem("1", "love", "movie", "offset=11&limit=65");
            item.Urls = new List<string>();
            item.Urls.Add(@"/search/movie/love/results?page=0");
            item.Urls.Add(@"/search/movie/love/results?page=1");
            item.Urls.Add(@"/search/movie/love/results?page=2");
            item.Urls.Add(@"/search/movie/love/results?page=3");
            IMetacriticData resp = item.Parse(pair);

            CollectionAssert.AllItemsAreInstancesOfType(((SearchData)resp).SearchItems,
                typeof(SearchData.SearchItem));
            Assert.IsNull(((SearchData)resp).TotalResultCount);
            Assert.AreEqual(((SearchData)resp).SearchItems.Count, 15);
            Assert.AreEqual(((SearchData)resp).SearchItems[2].Title, "A Lot Like Love");
            Assert.AreEqual(((SearchData)resp).SearchItems[6].Rating.CriticRating, 33);
            Assert.AreEqual(((SearchData)resp).SearchItems[8].ReleaseDate, "October 19, 2007");
            Assert.AreEqual(((SearchData)resp).SearchItems[13].Id, "/movie/my-summer-of-love");
        }

        [Test]
        public void Test_Search_Parse_Album()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            UrlResponsePair pair = new UrlResponsePair(@"/search/album/futire/results?page=0",
                File.ReadAllText(dir + @"\TestData\search_album_future.txt"), 1, 20);

            SearchRequestItem item = new SearchRequestItem("1", "future", "album", "");
            item.Urls = new List<string>();
            item.Urls.Add(@"/search/album/future/results?page=0");
            IMetacriticData resp = item.Parse(pair);

            CollectionAssert.AllItemsAreInstancesOfType(((SearchData)resp).SearchItems,
                typeof(SearchData.SearchItem));
            Assert.AreEqual(((SearchData)resp).TotalResultCount, 58);
            Assert.AreEqual(((SearchData)resp).SearchItems.Count, 20);
            Assert.AreEqual(((SearchData)resp).SearchItems[2].Id, "/album/future-present-past-ep");
            Assert.AreEqual(((SearchData)resp).SearchItems[6].Id, "/album/barbara-barbara-we-face-a-shining-future");
            Assert.AreEqual(((SearchData)resp).SearchItems[8].Id, "/album/love-in-the-future");
            Assert.AreEqual(((SearchData)resp).SearchItems[13].Id, "/album/in-the-future");
            Assert.AreEqual(((SearchData)resp).SearchItems[17].Id, "/album/future-standards");
        }
    }
}
