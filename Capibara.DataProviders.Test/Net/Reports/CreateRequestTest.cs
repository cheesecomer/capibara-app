using System.Net.Http;

using Capibara.Net.Reports;
using Capibara.Models;

using NUnit.Framework;

namespace Capibara.Test.Net.Reports
{
    namespace ExecuteTest
    {
        public abstract class TestBase
        {
            protected CreateRequest Actual { get; set; }

            [TestCase]
            public void ItShouldRequestWithHttpMethodPost()
            {
                Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Post));
            }

            [TestCase]
            public void ItShouldPathsWithExpect()
            {
                Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "reports" }));
            }

            [TestCase]
            public void ItShouldNeedAuthentication()
            {
                Assert.That(this.Actual.NeedAuthentication, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenMessageEmpty : TestBase
        {
            [SetUp]
            public void SetUp()
            {
                this.Actual = new CreateRequest(new User { Id = 100 }, ReportReason.Spam);
            }
            
            [TestCase]
            public void ItShouldStringContentWithExpected()
            {
                var expected = "{\"target_id\":100,\"reason\":1}".ToSlim();
                Assert.That(Actual.StringContent.ToSlim(), Is.EqualTo(expected));
            }
        }

        [TestFixture]
        public class WhenMessagePresent : TestBase
        {
            [SetUp]
            public void SetUp()
            {
                this.Actual = new CreateRequest(new User { Id = 100 }, ReportReason.Other, "FooBar");
            }

            [TestCase]
            public void ItShouldStringContentWithExpected()
            {
                var expected = "{\"target_id\":100,\"reason\":0,\"message\":\"FooBar\"}".ToSlim();
                Assert.That(Actual.StringContent.ToSlim(), Is.EqualTo(expected));
            }
        }
    }
}
