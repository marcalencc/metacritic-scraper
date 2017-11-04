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
        public void Test_UrlParser_ParseInvalidYearOrSeason()
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

        public void Test_UrlParser_ParseInvalidThirdLevelRequest()
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

        public void Test_UrlParser_ParseInvalidThirdLevelRequestWithSeason()
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
        public void Test_UrlParser_ParseInvalidKeyword()
        {
            string url = "/tvguide/veep/5";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, false);
            Assert.AreEqual(keyword, "");
            Assert.AreEqual(title, "");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void Test_UrlParser_ParseValidTvShowUrlWithSeasonAndDetails()
        {
            string url = "/tvshow/veep/3/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "veep");
            Assert.AreEqual(yearOrSeason, "3");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void Test_UrlParser_ParseValidTvShowUrlWithDetails()
        {
            string url = "/tvshow/veep/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "veep");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void Test_UrlParser_ParseValidTvShowUrlWithSeason()
        {
            string url = "/tvshow/veep/5";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "veep");
            Assert.AreEqual(yearOrSeason, "5");
        }

        [Test]
        public void Test_UrlParser_ParseValidTvShowUrl()
        {
            string url = "/tvshow/game-of-thrones";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/tvshow/");
            Assert.AreEqual(title, "game-of-thrones");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void Test_UrlParser_ParseValidMovieUrlWithDetails()
        {
            string url = "/movie/the-master/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "the-master");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "details");
        }


        [Test]
        public void Test_UrlParser_ParseValidMovieUrlWithYearAndDetails()
        {
            string url = "/movie/the-master/2012/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "the-master");
            Assert.AreEqual(yearOrSeason, "2012");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void Test_UrlParser_ParseValidMovieUrlWithYear()
        {
            string url = "/movie/the-master/2012";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "the-master");
            Assert.AreEqual(yearOrSeason, "2012");
        }

        [Test]
        public void Test_UrlParser_ParseValidMovieUrl()
        {
            string url = "/movie/guardians-of-the-galaxy";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/movie/");
            Assert.AreEqual(title, "guardians-of-the-galaxy");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void Test_UrlParser_ParseValidAlbumUrlWithYearAndDetails()
        {
            string url = "/album/to-pimp-a-butterfly/2015/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "to-pimp-a-butterfly");
            Assert.AreEqual(yearOrSeason, "2015");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void Test_UrlParser_ParseValidAlbumUrlWithDetails()
        {
            string url = "/album/to-pimp-a-butterfly/details";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "to-pimp-a-butterfly");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "details");
        }

        [Test]
        public void Test_UrlParser_ParseValidAlbumUrlWithYear()
        {
            string url = "/album/to-pimp-a-butterfly/2015";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "to-pimp-a-butterfly");
            Assert.AreEqual(yearOrSeason, "2015");
        }

        [Test]
        public void Test_UrlParser_ParseValidAlbumUrl()
        {
            string url = "/album/the-suburbs";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "the-suburbs");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void Test_UrlParser_ParseValidPersonUrl()
        {
            string url = "/person/martin-scorsese/album";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/person/");
            Assert.AreEqual(title, "martin-scorsese");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "album");
        }

        [Test]
        public void Test_UrlParser_ParseInvalidPersonUrlNoThirdLevelRequest()
        {
            string url = "/person/martin-scorsese";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo(@"Category required for ""person"" request"));
        }

        [Test]
        public void Test_UrlParser_ParseValidPersonUrlnvalidThirdLevelRequest()
        {
            string url = "/person/martin-scorsese/1945";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid parameter: 1945"));
        }

        [Test]
        public void Test_UrlParser_ParseValidUrlWithExtraSlash()
        {
            string url = "/album/the-suburbs/";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "the-suburbs");
            Assert.AreEqual(yearOrSeason, "");
        }

        [Test]
        public void Test_UrlParser_ParseValidUrlWithExtraSlash2()
        {
            string url = "/album/the-suburbs/details/";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "the-suburbs");
            Assert.AreEqual(yearOrSeason, "");
            Assert.AreEqual(thirdLevelReq, "details");
        }


        [Test]
        public void Test_UrlParser_ParseValidUrlWithExtraSlash3()
        {
            string url = "/album/the-suburbs/2011/";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/album/");
            Assert.AreEqual(title, "the-suburbs");
            Assert.AreEqual(yearOrSeason, "2011");
            Assert.AreEqual(thirdLevelReq, "");
        }

        [Test]
        public void Test_UrlParser_ParseInvalidUrlWithExtraSlash()
        {
            string url = "/album/";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, false);
        }

        [Test]
        public void Test_UrlParser_ParseInvalidUrlWithExtraSlash2()
        {
            string url = "/album/the-suburbs//";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid parameter: "));
        }

        [Test]
        public void Test_UrlParser_MovieItemCreationNoThirdLevelRequest()
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
        public void Test_UrlParser_MovieItemCreationWithYearAndThirdLevelRequest()
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
        public void Test_UrlParser_MovieItemCreationWithYearNoThirdLevelRequest()
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
        public void Test_UrlParser_AlbumItemCreationNoThirdLevelRequest()
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
        public void Test_UrlParser_AlbumItemCreationWithYearNoThirdLevelRequest()
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
        public void Test_UrlParser_AlbumItemCreationWithYearAndThirdLevelRequest()
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
        public void Test_UrlParser_TvShowItemCreationWithThirdLevelRequest()
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
        public void Test_UrlParser_TvShowItemCreationNoThirdLevelRequest()
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
        public void Test_UrlParser_PersonItemCreationNoThirdLevelRequest()
        {
            string keyword = "/person/";
            string title = "lorde";
            string yearOrSeason = "";

            Assert.That(() => m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, ""),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo(@"Category required for ""person"" request"));
        }

        [Test]
        public void Test_UrlParser_PersonItemCreation()
        {
            string keyword = "/person/";
            string title = "justin-bieber";
            string thirdLevelReq = "album";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, "", thirdLevelReq);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(PersonRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.PersonTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "justin bieber");
            Assert.AreEqual(item.ThirdLevelRequest, "album");
        }

        [Test]
        public void Test_UrlParser_ItemCreationWithDash()
        {
            string keyword = "/movie/";
            string title = @"i-am-e~-mo~-tion~-al-sometimes";
            string yearOrSeason = "2012";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, "");

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(MovieRequestItem), item);
            Assert.AreEqual(item.RefTypeId, Constants.MovieTypeId);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "i am e-mo-tion-al sometimes");
            Assert.AreEqual(item.ItemDate, "2012");
        }

        [Test]
        public void Test_UrlParser_InvalidKeyword()
        {
            string keyword = "/magazine/";
            string title = "2-broke-girls";
            string yearOrSeason = "1";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, "");

            Assert.IsNull(item);
        }
    }
}
