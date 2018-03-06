using System.Net.Http;

using Capibara.Models;
using Capibara.Net.Blocks;

using NUnit.Framework;

namespace Capibara.Test.Net.Blocks.DestroyRequestTest
{
    [TestFixture]
    public class ExecuteTest
    {
        private DestroyRequest Actual { get; set; }

        [SetUp]
        public void SetUp()
        {
            this.Actual = new DestroyRequest(new Block { Id = 1 });
        }

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Delete));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "blocks", "1" }));
        }
    }
}

