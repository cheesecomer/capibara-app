using System.Net.Http;

using Capibara.Net.DirectMessages;

using NUnit.Framework;

namespace Capibara.Test.Net.DirectMessages
{
    [TestFixture]
    public class IndexRequestTest
    {
        private IndexRequest Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new IndexRequest();
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Get));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "direct_messages" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Subject.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
