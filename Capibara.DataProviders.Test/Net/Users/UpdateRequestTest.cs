using System;
using System.Net.Http;
using System.IO;


using Capibara.Models;
using Capibara.Net.Users;

using NUnit.Framework;

namespace Capibara.Test.Net.Users
{
    public class UpdateRequestTest
    {
        private UpdateRequest request;

        [SetUp]
        public void SetUp()
        {
            this.request = new UpdateRequest(new User { Id = 1000 });
        }

        [TestCase]
        public void ItShouldHttMethodToGet()
        {
            Assert.That(this.request.Method, Is.EqualTo(HttpMethod.Put));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.request.Paths, Is.EqualTo(new[] { "users" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.request.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
