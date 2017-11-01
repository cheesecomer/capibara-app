using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

using Capibara.Net;
using Capibara.Net.Sessions;

using NUnit.Framework;
using Moq;

namespace Capibara.Test.Net.Sessions.CreateRequestTest
{
    [TestFixture]
    public class ExecuteTest : TestFixtureBase
    {
        private CreateRequest Actual { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Actual = new CreateRequest()
            {
                Email = "user@email.com",
                Password = "p@ssword"
            };
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "session" }));
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"email\":\"user@email.com\",\"password\":\"p@ssword\"}".ToSlim();
            Assert.That(Actual.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }
}
