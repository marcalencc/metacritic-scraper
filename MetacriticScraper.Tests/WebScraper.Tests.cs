using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using MetacriticScraper.Scraper;
using MetacriticScraper.Errors;

namespace MetacriticScraper.Tests
{
    [TestFixture]
    public class WebScraperTest
    {
        private WebScraper _metacriticScraper;

        [OneTimeSetUp]
        public void Setup()
        {
            _metacriticScraper = new WebScraper(null, 10);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _metacriticScraper = null;
        }

        [Test]
        public void TestParseInvalidMovieItemYear()
        {
            string id = "1";
            string url = "/movie/there-will-be-blood/etc";

            Assert.That(() => _metacriticScraper.AddItem(id, url),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid year or season value"));
        }

        [Test]
        public void TestParseInvalidAlbumItemYear()
        {
            string id = "1";
            string url = "/album/lemonade/etc";

            Assert.That(() => _metacriticScraper.AddItem(id, url),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid year or season value"));
        }

        [Test]
        public void TestParseInvalidTvShowItemSeason()
        {
            string id = "1";
            string url = "/tvshow/30-rock/etc";

            Assert.That(() => _metacriticScraper.AddItem(id, url),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Invalid year or season value"));
        }

        [Test]
        public void TestParseInvalidUrlFormat()
        {
            string id = "1";
            string url = "/magazine/30-rock";

            Assert.That(() => _metacriticScraper.AddItem(id, url),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo("Url has invalid format"));
        }

        [Test]
        public void TestSystemBusy()
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
