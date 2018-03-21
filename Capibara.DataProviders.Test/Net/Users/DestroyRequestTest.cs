using System;
using System.Net.Http;

using Capibara.Models;
using Capibara.Net.Users;

using NUnit.Framework;

namespace Capibara.Test.Net.Users
{
    public class DestroyRequestTest
    {
        private DestroyRequest Subject;

        public DestroyRequestTest()
        {
            this.Subject = new DestroyRequest();
        }

        [TestCase]
        public void ItShouldHttMethodToGet()
        {
            Assert.That(this.Subject.Method, Is.EqualTo(HttpMethod.Delete));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.Subject.Paths, Is.EqualTo(new[] { "users" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.Subject.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
