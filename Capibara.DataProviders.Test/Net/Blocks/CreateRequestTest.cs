using System.Net.Http;

using Capibara.Models;
using Capibara.Net.Blocks;

using NUnit.Framework;

namespace Capibara.Test.Net.Blocks.CreateRequestTest
{
    [TestFixture]
    public class ExecuteTest
    {
        private CreateRequest Actual { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Actual = new CreateRequest(new User { Id = 1 });
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "blocks" }));
        }

        [TestCase]
        public void ItShouldStringContentWithExpected()
        {
            var expected = "{\"target_id\":1}".ToSlim();
            Assert.That(Actual.StringContent.ToSlim(), Is.EqualTo(expected));
        }
    }
}
