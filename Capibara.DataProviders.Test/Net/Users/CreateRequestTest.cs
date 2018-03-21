using System.Net.Http;

using Capibara.Net.Users;

using NUnit.Framework;

namespace Capibara.Test.Net.Users.CreateRequestTest
{
    [TestFixture]
    public class ExecuteTest
    {
        private CreateRequest Subject { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Subject = new CreateRequest("Foo.BAR");
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "users" }));
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"nickname\":\"Foo.BAR\"}".ToSlim();
            Assert.That(Subject.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }
}
