using System.Net.Http;

using Capibara.Models;
using Capibara.Net.Blocks;

using NUnit.Framework;

namespace Capibara.Test.Net.Blocks
{
    [TestFixture]
    public class DestroyRequestTest
    {
        private DestroyRequest Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new DestroyRequest(1);
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Delete));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "blocks", "1" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Subject.NeedAuthentication, Is.EqualTo(true));
        }
    }
}

