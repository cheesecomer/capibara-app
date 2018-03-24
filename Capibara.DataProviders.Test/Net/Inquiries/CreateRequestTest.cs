using System.Net.Http;

using Capibara.Net.Inquiries;

using NUnit.Framework;

namespace Capibara.Test.Net.Inquiries
{
    [TestFixture]
    public class CreateRequestTest
    {
        private CreateRequest Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new CreateRequest("example@email.com", "Message!");
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "inquiries" }));
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"email\": \"example @email.com\", \"content\":\"Message!\"}".ToSlim();
            Assert.That(Subject.StringContent.ToSlim(), Is.EqualTo(expected));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Subject.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
