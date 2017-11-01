using System.Net.Http;

using Capibara.Net.Users;

using NUnit.Framework;

namespace Capibara.Test.Net.Users.CreateRequestTest
{
    [TestFixture]
    public class ExecuteTest
    {
        private CreateRequest Actual { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Actual = new CreateRequest { Nickname = "Foo.BAR" };
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "users" }));
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"nickname\":\"Foo.BAR\"}".ToSlim();
            Assert.That(Actual.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }
}
