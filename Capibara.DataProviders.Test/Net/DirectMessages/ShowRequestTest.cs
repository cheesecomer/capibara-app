using System.Collections.Generic;
using System.Net.Http;

using Capibara.Models;
using Capibara.Net.DirectMessages;

using NUnit.Framework;

namespace Capibara.Test.Net.DirectMessages.ShowRequestTest
{
    public abstract class TestFixtureBase
    {
        protected ShowRequest Subject { get; set; }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Get));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "direct_messages", "1" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Subject.NeedAuthentication, Is.EqualTo(true));
        }
    }

    [TestFixture]
    public class WhenHasNotLastId : TestFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Subject = new ShowRequest(new User { Id = 1 });
        }

        [TestCase]
        public void ItShouldQueryEmpty()
        {
            Assert.That(this.Subject.Query.Count, Is.EqualTo(0));
        }
    }

    public class WhenHasLastId : TestFixtureBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Subject = new ShowRequest(new User { Id = 1 }, 1000);
        }

        [TestCase]
        public void ItShouldQueryExpect()
        {
            Assert.That(
                this.Subject.Query,
                Is.EqualTo(new Dictionary<string, string> { { "last_id", "1000" } }));
        }
    }
}
