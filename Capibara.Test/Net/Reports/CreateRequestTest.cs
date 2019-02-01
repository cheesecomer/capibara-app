using System.Net.Http;

using Capibara.Net.Reports;
using Capibara.Models;

using NUnit.Framework;

namespace Capibara.Test.Net.Reports.CreateRequestTest
{
    public abstract class TestBase
    {
        protected CreateRequest Subject { get; set; }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "reports" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Subject.NeedAuthentication, Is.EqualTo(true));
        }
    }

    [TestFixture]
    public class WhenMessageEmpty : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Subject = new CreateRequest(new User { Id = 100 }, ReportReason.Spam);
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"target_id\":100,\"reason\":1}".ToSlim();
            Assert.That(Subject.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }

    [TestFixture]
    public class WhenMessagePresent : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Subject = new CreateRequest(new User { Id = 100 }, ReportReason.Other, "FooBar");
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"target_id\":100,\"reason\":0,\"message\":\"FooBar\"}".ToSlim();
            Assert.That(Subject.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }
}
