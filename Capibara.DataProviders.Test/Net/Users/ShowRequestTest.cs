using System;
using System.Net.Http;
using System.IO;


using Capibara.Models;
using Capibara.Net.Users;

using NUnit.Framework;

namespace Capibara.Test.Net.Users
{
    [TestFixture]
    public class ShowRequestTest
    {
        private ShowRequest request;

        public ShowRequestTest()
        {
            this.request = new ShowRequest(new User { Id = 1000 });
        }

        [TestCase]
        public void ItShouldHttMethodToGet()
        {
            Assert.That(this.request.Method, Is.EqualTo(HttpMethod.Get));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(this.request.Paths, Is.EqualTo(new[] { "users", "1000" }));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.request.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
