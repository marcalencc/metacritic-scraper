using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MetacriticScraper.Scraper;
using MetacriticScraper.Errors;
using MetacriticScraper.RequestData;

namespace MetacriticScraper.Tests
{
    [TestFixture]
    public class UrlParserTest
    {
        private UrlParser m_urlParser;

        [OneTimeSetUp]
        public void Setup()
        {
            m_urlParser = new UrlParser();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            m_urlParser = null;
        }

        [Test]
        public void ParseInvalidYearOrSeason()
        {
            string url = "/tvshow/veep/yolo";
            string keyword;
            string title;
            string yearOrSeason;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid year or season value"));
        }

        [Test]
        public void ParseInvalidKeyword()
        {
            string url = "/tvguide/veep/5";
            string keyword;
            string title;
            string yearOrSeason;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason);

            Assert.AreEqual(keyword, "");
            Assert.AreEqual(title, "");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void ParseValidTvShowUrlWithSeason()
        {
            string url = "/tvshow/veep/5";
            string keyword;
            string title;
            string yearOrSeason;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason);

            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "veep");
            Assert.AreEqual(yearOrSeason, "5");
        }

        [Test]
        public void ParseValidTvShowUrl()
        {
            string url = "/tvshow/game-of-thrones";
            string keyword;
            string title;
            string yearOrSeason;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason);

            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "game-of-thrones");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void ParseValidMovieUrlWithYear()
        {
            string url = "/movie/the-master/2012";
            string keyword;
            string title;
            string yearOrSeason;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason);

            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "the-master");
            Assert.AreEqual(yearOrSeason, "2012");
        }

        [Test]
        public void ParseValidMovieUrl()
        {
            string url = "/movie/guardians-of-the-galaxy";
            string keyword;
            string title;
            string yearOrSeason;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason);

            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "guardians-of-the-galaxy");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void ParseValidAlbumUrlWithYear()
        {
            string url = "/album/to-pimp-a-butterfly/2015";
            string keyword;
            string title;
            string yearOrSeason;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason);

            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "to-pimp-a-butterfly");
            Assert.AreEqual(yearOrSeason, "2015");
        }

        [Test]
        public void ParseValidAlbumUrl()
        {
            string url = "/album/the-suburbs";
            string keyword;
            string title;
            string yearOrSeason;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason);

            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "the-suburbs");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void TestMovieItemCreation()
        {
            string keyword = "/movie/";
            string title = "die-another-day";
            string yearOrSeason = "";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(MovieRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.MovieTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "die another day");
        }

        [Test]
        public void TestMovieItemCreationWithYear()
        {
            string keyword = "/movie/";
            string title = "la-la-land";
            string yearOrSeason = "2016";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(MovieRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.MovieTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "la la land");
            Assert.AreEqual(item.ItemDate, "2016");
        }

        [Test]
        public void TestAlbumItemCreation()
        {
            string keyword = "/album/";
            string title = "views";
            string yearOrSeason = "";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(AlbumRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.AlbumTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "views");
        }

        [Test]
        public void TestAlbumItemCreationWithYear()
        {
            string keyword = "/album/";
            string title = "melodrama";
            string yearOrSeason = "2017";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(AlbumRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.AlbumTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "melodrama");
            Assert.AreEqual(item.ItemDate, "2017");
        }

        [Test]
        public void TestTvShowItemCreation()
        {
            string keyword = "/tvshow/";
            string title = "olive-kitteridge";
            string yearOrSeason = "";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(TVShowRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.TvShowTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "olive kitteridge");
        }

        [Test]
        public void TestInvalidKeyword()
        {
            string keyword = "/magazine/";
            string title = "2-broke-girls";
            string yearOrSeason = "1";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason);

            Assert.IsNull(item);
        }
    }
}
