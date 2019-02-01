using System;
using System.Net.Http;
using System.IO;

using Capibara.Models;
using Capibara.Net.Rooms;

using NUnit.Framework;

namespace Capibara.Test.Net.Rooms
{
    public class IndexRequestTest
    {
        private IndexRequest request;

        public IndexRequestTest()
        {
            this.request = new IndexRequest();
        }

        [TestCase]
        public void ItShouldHttMethodToGet()
        {
            Assert.That(this.request.Method, Is.EqualTo(HttpMethod.Get));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(Path.Combine(this.request.Paths), Is.EqualTo("rooms"));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.request.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
