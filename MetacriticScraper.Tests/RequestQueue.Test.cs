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
    public class RequestQueueTest
    {
        private RequestQueue<object> queue;

        [SetUp]
        public void Setup()
        {
            queue = new RequestQueue<object>(2);
        }

        [TearDown]
        public void TearDown()
        {
            queue = null;
        }

        [Test]
        public void Test_RequestQueue_Enqueuing()
        {
            queue.Enqueue(new object());

            Assert.AreEqual(queue.Count, 1);
        }

        [Test]
        public void Test_RequestQueue_Dequeuing()
        {
            queue.Enqueue(new object());
            queue.Enqueue(new object());
            object obj = queue.Dequeue();

            Assert.AreEqual(queue.Count, 1);
            Assert.IsNotNull(obj);
        }

        [Test]
        public void Test_RequestQueue_QueueLimit()
        {
            queue.Enqueue(new object());
            queue.Enqueue(new object());
            Assert.IsFalse(queue.HasAvailableSlot());
        }
    }
}
