using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

using Capibara.Net.Sessions;

using NUnit.Framework;
using Moq;

namespace Capibara.Test.Net.Sessions
{
    [TestFixture]
    public class CreateRequestTest : TestFixtureBase
    {
        private CreateRequest Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new CreateRequest("user@email.com", "p@ssword");
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "session" }));
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"email\":\"user@email.com\",\"password\":\"p@ssword\"}".ToSlim();
            Assert.That(Subject.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }
}
