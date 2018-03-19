using System;
using System.Net.Http;

using Capibara.Models;
using Capibara.Net.Users;

using NUnit.Framework;

namespace Capibara.Test.Net.Users
{
    public class DestroyRequestTest
    {
        private DestroyRequest Actual;

        public DestroyRequestTest()
        {
            this.Actual = new DestroyRequest();
        }

        [TestCase]
        public void ItShouldHttMethodToGet()
        {
            Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Delete));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "users" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Actual.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
