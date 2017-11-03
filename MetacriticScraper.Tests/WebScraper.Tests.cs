using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MetacriticScraper.Scraper;
using MetacriticScraper.Errors;
using MetacriticScraper.Interfaces;
using MetacriticScraper.RequestData;
using Moq;

namespace MetacriticScraper.Tests
{
    [TestFixture]
    public class WebScraperTest
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
        public void Test_WebScraper_ErrorWhenParsingUrl()
        {
            var parser = new Mock<IParser>();
            string dummyKeyword;
            string dummyTitle;
            string dummyYear;
            string thirdLevelReq;
            string param = null;

            parser.Setup(p => p.ParseRequestUrl(It.IsAny<string>(), It.IsAny<string>(), out dummyKeyword,
                out dummyTitle, out dummyYear, out thirdLevelReq, ref param)).Throws(new InvalidUrlException("Invalid year or season value"));
            parser.Setup(p => p.CreateRequestItem(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(
                new MovieRequestItem("id", "title", ""));

            IScraper scraper = new WebScraper(null, 10); 
            scraper.UrlParser = parser.Object;

            Assert.That(() => scraper.AddItem("id", "url"),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid year or season value"));
        }

        [Test]
        public void Test_WebScraper_ErrorWhenCreatingRequestData()
        {
            var parser = new Mock<IParser>();
            string dummyKeyword;
            string dummyTitle;
            string dummyYear = "2012";
            string thirdLevelReq;
            string param = null;

            parser.Setup(p => p.ParseRequestUrl(It.IsAny<string>(), It.IsAny<string>(), out dummyKeyword,
                out dummyTitle, out dummyYear, out thirdLevelReq, ref param)).Returns(true);
            parser.Setup(p => p.CreateRequestItem(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).
                Throws(new InvalidUrlException("Invalid year or season value"));

            IScraper scraper = new WebScraper(null, 10);
            scraper.UrlParser = parser.Object;

            Assert.That(() => scraper.AddItem("id", "url"),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid year or season value"));
        }

        [Test]
        public void Test_WebScraper_SuccessfulAddProcess()
        {
            var parser = new Mock<IParser>();
            string dummyKeyword;
            string dummyTitle;
            string dummyYear = "2012";
            string thirdLevelReq;
            string param = null;

            parser.Setup(p => p.ParseRequestUrl(It.IsAny<string>(), It.IsAny<string>(), out dummyKeyword,
                out dummyTitle, out dummyYear, out thirdLevelReq, ref param)).Returns(true);
            parser.Setup(p => p.CreateRequestItem(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(
                new MovieRequestItem("id", "title", ""));

            IScraper scraper = new WebScraper(null, 10);
            scraper.UrlParser = parser.Object;

            Assert.IsTrue(scraper.AddItem("id", "url"));
        }

        [Test]
        public void Test_WebScraper_SystemBusy()
        {
            string id = "1";
            string url = "/tvshow/30-rock/6";

            WebScraper _metacriticScraperZeroCapacity = new WebScraper(null, 0);

            Assert.That(() => _metacriticScraperZeroCapacity.AddItem(id, url),
                Throws.Exception.TypeOf<SystemBusyException>().
                With.Property("Message").
                EqualTo("Too many requests at the moment"));
        }
    }
}
