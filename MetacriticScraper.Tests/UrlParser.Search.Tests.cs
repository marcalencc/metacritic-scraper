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
    public class UrlParserSearchTest
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
        public void Test_UrlParser_Search_ParseValidUrlNoParameterSet()
        {
            string url = "/search/hark-lark/album";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/search/");
            Assert.AreEqual(title, "hark-lark");
            Assert.AreEqual(thirdLevelReq, "album");
        }

        [Test]
        public void Test_UrlParser_Search_ParseValidUrlWithParameterSet()
        {
            string url = "/search/lover/movie?offset=3&limit=1&sort=relevancy";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;
            string param = null;

            bool ret = m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq, ref param);

            Assert.AreEqual(ret, true);
            Assert.AreEqual(keyword, "/search/");
            Assert.AreEqual(title, "lover");
            Assert.AreEqual(thirdLevelReq, "movie");
            Assert.AreEqual(param, "offset=3&limit=1&sort=relevancy" );
        }

        [Test]
        public void Test_UrlParser_Search_ParseInvalidTooShortTitle()
        {
            string url = "/search/lo/movie?offset=3&limit=1&sort=relevancy";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;
            string param = null;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq, ref param), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo(@"Search query should be at least three characters."));
        }

        [Test]
        public void Test_UrlParser_Search_ParseInvalidSearchUrlNoThirdLevelRequest()
        {
            string url = "/search/lover?offset=3&limit=1&sort=relevancy";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;
            string param = null;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq, ref param), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo(@"Category required for ""search"" request"));
        }

        [Test]
        public void Test_UrlParser_Search_ParseInvalidSearchUrlInvalidThirdLevelRequest()
        {
            string url = "/search/lover/dvr?offset=3&limit=1&sort=relevancy";
            string keyword;
            string title;
            string yearOrSeason;
            string thirdLevelReq;
            string param = null;

            Assert.That(() => m_urlParser.ParseRequestUrl("1", url, out keyword, out title, out yearOrSeason,
                out thirdLevelReq, ref param), Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo(@"Invalid parameter: dvr?offset=3&limit=1&sort=relevancy"));
        }

        [Test]
        public void Test_UrlParser_SearchItemCreationNoThirdLevelRequest()
        {
            string keyword = "/search/";
            string title = "lover";
            string yearOrSeason = "";

            Assert.That(() => m_urlParser.CreateRequestItem("1", keyword, title, yearOrSeason, ""),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo(@"Category required for ""search"" request"));
        }

        [Test]
        public void Test_UrlParser_SearchItemCreation()
        {
            string keyword = "/search/";
            string title = "love-game";
            string thirdLevelReq = "album";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, "", thirdLevelReq);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(SearchRequestItem), item);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(item.Name, "love game");
            Assert.AreEqual(item.ThirdLevelRequest, "album");
        }

        [Test]
        public void Test_UrlParser_ItemCreationWithParameters()
        {
            string keyword = "/search/";
            string title = @"game";
            string thirdLevelReq = "album";
            string parameter = "offset=3&limit=1&sort=relevancy";

            RequestItem item = m_urlParser.CreateRequestItem("1", keyword, title, "",
                thirdLevelReq, parameter);

            Assert.IsNotNull(item);
            Assert.IsInstanceOf(typeof(SearchRequestItem), item);
            Assert.AreEqual(item.RequestId, "1");
            Assert.AreEqual(((SearchRequestItem)item).ParameterData.ParameterString,
                "offset=3&limit=1&sort=relevancy");
        }
    }
}
