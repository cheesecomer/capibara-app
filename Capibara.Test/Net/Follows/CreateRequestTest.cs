using System.Net.Http;

using Capibara.Models;
using Capibara.Net.Follows;

using NUnit.Framework;

namespace Capibara.Test.Net.Follows
{
    [TestFixture]
    public class CreateRequestTest
    {
        private CreateRequest Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new CreateRequest(new User { Id = 1 });
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "follows" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Subject.NeedAuthentication, Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"target_id\":1}".ToSlim();
            Assert.That(Subject.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }
}
