using System;
using System.Net.Http;
using System.IO;

using Capibara.Net.Follows;

using NUnit.Framework;

namespace Capibara.Test.Net.Follows
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
            Assert.That(Path.Combine(this.request.Paths), Is.EqualTo("follows"));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.request.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
