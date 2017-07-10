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
            string url = "/tvshow/veep/yolo/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid year or season value"));
        }

        public void ParseInvalidThirdLevelRequest()
        {
            string url = "/tvshow/veep/detailor";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid parameter: detailor"));
        }

        public void ParseInvalidThirdLevelRequestWithSeason()
        {
            string url = "/tvshow/veep/3/detailor";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid parameter: detailor"));
        }

        [Test]
        public void ParseInvalidKeyword()
        {
            string url = "/tvguide/veep/5";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "");
            Assert.AreEqual(title, "");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void ParseValidTvShowUrlWithSeasonAndDetails()
        {
            string url = "/tvshow/veep/3/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "veep");
            Assert.AreEqual(yearOrSeason, "3");
            Assert.AreEqual(thirdLevelReq, "details");
        }


        [Test]
        public void ParseValidTvShowUrlWithDetails()
        {
            string url = "/tvshow/veep/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "veep");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void ParseValidTvShowUrlWithSeason()
        {
            string url = "/tvshow/veep/5";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

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
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "game-of-thrones");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void ParseValidMovieUrlWithDetails()
        {
            string url = "/movie/the-master/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "the-master");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "details");
        }


        [Test]
        public void ParseValidMovieUrlWithYearAndDetails()
        {
            string url = "/movie/the-master/2012/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "the-master");
            Assert.AreEqual(yearOrSeason, "2012");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void ParseValidMovieUrlWithYear()
        {
            string url = "/movie/the-master/2012";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

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
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "guardians-of-the-galaxy");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void ParseValidAlbumUrlWithYearAndDetails()
        {
            string url = "/album/to-pimp-a-butterfly/2015/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "to-pimp-a-butterfly");
            Assert.AreEqual(yearOrSeason, "2015");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void ParseValidAlbumUrlWithDetails()
        {
            string url = "/album/to-pimp-a-butterfly/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "to-pimp-a-butterfly");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void ParseValidAlbumUrlWithYear()
        {
            string url = "/album/to-pimp-a-butterfly/2015";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

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
            string thirdLevelReq;

            m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "the-suburbs");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void TestMovieItemCreationNoThirdLevelRequest()
        {
            string keyword = "/movie/";
            string title = "die-another-day";
            string yearOrSeason = "";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, "");

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(MovieRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.MovieTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "die another day");
        }

        [Test]
        public void TestMovieItemCreationWithYearAndThirdLevelRequest()
        {
            string keyword = "/movie/";
            string title = "la-la-land";
            string yearOrSeason = "2016";
            string thirdLevelRequest = "details";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason,
                thirdLevelRequest);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(MovieRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.MovieTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "la la land");
            Assert.AreEqual(item.ItemDate, "2016");
            Assert.AreEqual(item.ThirdLevelRequest, "details");
        }

        [Test]
        public void TestMovieItemCreationWithYearNoThirdLevelRequest()
        {
            string keyword = "/movie/";
            string title = "la-la-land";
            string yearOrSeason = "2016";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, "");

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(MovieRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.MovieTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "la la land");
            Assert.AreEqual(item.ItemDate, "2016");
        }

        [Test]
        public void TestAlbumItemCreationNoThirdLevelRequest()
        {
            string keyword = "/album/";
            string title = "views";
            string yearOrSeason = "";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, "");

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(AlbumRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.AlbumTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "views");
        }

        [Test]
        public void TestAlbumItemCreationWithYearNoThirdLevelRequest()
        {
            string keyword = "/album/";
            string title = "melodrama";
            string yearOrSeason = "2017";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, "");

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(AlbumRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.AlbumTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "melodrama");
            Assert.AreEqual(item.ItemDate, "2017");
        }

        [Test]
        public void TestAlbumItemCreationWithYearAndThirdLevelRequest()
        {
            string keyword = "/album/";
            string title = "melodrama";
            string yearOrSeason = "2017";
            string thirdLevelRequest = "details";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason,
                thirdLevelRequest);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(AlbumRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.AlbumTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "melodrama");
            Assert.AreEqual(item.ItemDate, "2017");
            Assert.AreEqual(item.ThirdLevelRequest, "details");
        }

        [Test]
        public void TestTvShowItemCreationWithThirdLevelRequest()
        {
            string keyword = "/tvshow/";
            string title = "olive-kitteridge";
            string yearOrSeason = ""; ;
            string thirdLevelReq = "details";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason,
                thirdLevelReq);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(TVShowRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.TvShowTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "olive kitteridge");
            Assert.AreEqual(item.ThirdLevelRequest, "details");
        }

        [Test]
        public void TestTvShowItemCreationNoThirdLevelRequest()
        {
            string keyword = "/tvshow/";
            string title = "olive-kitteridge";
            string yearOrSeason = "";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason,"");

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

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, "");

            Assert.IsNull(item);
        }
    }
}
