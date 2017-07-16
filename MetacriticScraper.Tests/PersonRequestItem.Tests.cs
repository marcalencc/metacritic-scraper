using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using MetacriticScraper.Interfaces;
using MetacriticScraper.Errors;
using MetacriticScraper.RequestData;
using MetacriticScraper.JSONObjects;
using MetacriticScraper.MediaData;
using Moq;

namespace MetacriticScraper.Tests
{
    [TestFixture]
    public class PersonRequestItemTest
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
        public void Test_PersonRequestItem_PersonAlbumFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "album");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/person/woody-allen?filter-options=music&sort_options=date&num_items=100");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_PersonRequestItem_PersonMovieFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "movie");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/person/woody-allen?filter-options=movies&sort_options=date&num_items=100");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_PersonRequestItem_PersonTvShowFilterValidUrls()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "tvshow");
            item.AutoResult = completeData.AutoComplete.Results;
            bool result = item.FilterValidUrls();

            Assert.AreEqual(item.Urls.Count, 1);
            Assert.AreEqual(item.Urls[0], "/person/woody-allen?filter-options=tv&sort_options=date&num_items=100");
            Assert.IsTrue(result);
        }

        [Test]
        public void Test_PersonRequestItem_PersonFilterValidUrlsInvalid()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_autosearch.txt");
            var completeData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(testData);

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "person");
            item.AutoResult = completeData.AutoComplete.Results;

            Assert.That(() => item.FilterValidUrls(),
                Throws.Exception.TypeOf<InvalidUrlException>().
                With.Property("Message").
                EqualTo(@"Category required for ""person"" request"));
        }

        [Test]
        public void Test_PersonRequestItem_PersonScrape()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_movie_100_scraped.txt");

            var webUtils = new Mock<IWebUtils>();
            webUtils.Setup(p => p.HttpGet(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<int>())).Returns(Task.FromResult(testData));

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "movie");
            item.Urls = new List<string>();
            item.Urls.Add(@"/person/woody-allen?filter-options=movies&sort_options=date&num_items=100");
            item.WebUtils = webUtils.Object;
            List<string> resp = item.Scrape();

            Assert.AreEqual(resp.Count, 1);
        }

        [Test]
        public void Test_PersonRequestItem_MovieParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_movie_100_scraped.txt");

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "movie");
            MetacriticData mItem = item.Parse(testData);

            Assert.AreEqual(((Person)mItem).Name, "Woody Allen");
            Assert.AreEqual(((Person)mItem).RatingsSummary.HighestRating, 90);
            Assert.AreEqual(((Person)mItem).RatingsSummary.LowestRating, 32);
            Assert.AreEqual(((Person)mItem).RatingsSummary.ReviewCount, 36);
            Assert.AreEqual(((Person)mItem).CreditMediaPairItems.Count, 37);
            CollectionAssert.Contains(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item),
                new MediaItem() { Title = "Hannah and Her Sisters",
                    ReleaseDate = new System.DateTime(1986, 2, 1) } );
            CollectionAssert.AllItemsAreInstancesOfType(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item), typeof(MediaItem));
        }

        [Test]
        public void Test_PersonRequestItem_MovieParse2()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_kanyewest_movie_100_scraped.txt");

            PersonRequestItem item = new PersonRequestItem("1", "kanye west", "movie");
            MetacriticData mItem = item.Parse(testData);

            Assert.AreEqual(((Person)mItem).Name, "Kanye West");
            Assert.AreEqual(((Person)mItem).RatingsSummary.HighestRating, 81);
            Assert.AreEqual(((Person)mItem).RatingsSummary.LowestRating, 24);
            Assert.AreEqual(((Person)mItem).RatingsSummary.ReviewCount, 6);
            Assert.AreEqual(((Person)mItem).CreditMediaPairItems.Count, 7);
            CollectionAssert.Contains(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item),
                new MediaItem()
                {
                    Title = "Made in America",
                    ReleaseDate = new System.DateTime(2014, 7, 11)
                });
            CollectionAssert.AllItemsAreInstancesOfType(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item), typeof(MediaItem));
        }

        [Test]
        public void Test_PersonRequestItem_TvShowParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_tvshow_100_scraped.txt");

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "tvshow");
            MetacriticData mItem = item.Parse(testData);

            Assert.AreEqual(((Person)mItem).Name, "Woody Allen");
            Assert.AreEqual(((Person)mItem).RatingsSummary.HighestRating, 0);
            Assert.AreEqual(((Person)mItem).RatingsSummary.LowestRating, 0);
            Assert.AreEqual(((Person)mItem).RatingsSummary.ReviewCount, 0);
            Assert.AreEqual(((Person)mItem).CreditMediaPairItems.Count, 3);
            CollectionAssert.Contains(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item),
                new MediaItem()
                {
                    Title = "The Ed Sullivan Show: Season 22",
                    ReleaseDate = new System.DateTime(1969, 9, 28)
                });
            CollectionAssert.AllItemsAreInstancesOfType(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item), typeof(MediaItem));
        }

        [Test]
        public void Test_PersonRequestItem_TvShowParse2()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_kanyewest_tvshow_100_scraped.txt");

            PersonRequestItem item = new PersonRequestItem("1", "kanye west", "tvshow");
            MetacriticData mItem = item.Parse(testData);

            Assert.AreEqual(((Person)mItem).Name, "Kanye West");
            Assert.AreEqual(((Person)mItem).RatingsSummary.HighestRating, 0);
            Assert.AreEqual(((Person)mItem).RatingsSummary.LowestRating, 0);
            Assert.AreEqual(((Person)mItem).RatingsSummary.ReviewCount, 0);
            Assert.AreEqual(((Person)mItem).CreditMediaPairItems.Count, 7);
            CollectionAssert.Contains(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item),
                new MediaItem()
                {
                    Title = "Kourtney and Kim Take New York: Season 1",
                    ReleaseDate = new System.DateTime(2011, 1, 23)
                });
            CollectionAssert.AllItemsAreInstancesOfType(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item), typeof(MediaItem));
        }

        [Test]
        public void Test_PersonRequestItem_AlbumParse()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_kanyewest_album_100_scraped.txt");

            PersonRequestItem item = new PersonRequestItem("1", "kanye west", "album");
            MetacriticData mItem = item.Parse(testData);

            Assert.AreEqual(((Person)mItem).Name, "Kanye West");
            Assert.AreEqual(((Person)mItem).RatingsSummary.HighestRating, 94);
            Assert.AreEqual(((Person)mItem).RatingsSummary.LowestRating, 66);
            Assert.AreEqual(((Person)mItem).RatingsSummary.ReviewCount, 9);
            Assert.AreEqual(((Person)mItem).CreditMediaPairItems.Count, 9);

            CollectionAssert.Contains(((Person)mItem).CreditMediaPairItems.
            Select(c => c.Item),
            new MediaItem()
            {
                Title = "My Beautiful Dark Twisted Fantasy",
                ReleaseDate = new System.DateTime(2010, 11, 22)
            });

            CollectionAssert.AllItemsAreInstancesOfType(((Person)mItem).CreditMediaPairItems.
                Select(c => c.Item), typeof(MediaItem));
        }

        [Test]
        public void Test_PersonRequestItem_AlbumParse2()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string testData = File.ReadAllText(dir + @"\TestData\person_woodyallen_tvshow_100_scraped.txt");

            PersonRequestItem item = new PersonRequestItem("1", "woody allen", "album");
            MetacriticData mItem = item.Parse(testData);

            Assert.AreEqual(((Person)mItem).Name, "Woody Allen");
            Assert.AreEqual(((Person)mItem).RatingsSummary, default(Person.PersonRatingSummary));
            Assert.AreEqual(((Person)mItem).CreditMediaPairItems, null);
        }
    }
}
