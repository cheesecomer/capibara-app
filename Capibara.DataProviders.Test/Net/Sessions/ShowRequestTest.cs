using System;
using System.Net.Http;
using System.IO;

using Capibara.Models;
using Capibara.Net.Sessions;

using NUnit.Framework;

namespace Capibara.Test.Net.Sessions
{
    public class ShowRequestTest
    {
        private ShowRequest request;

        public ShowRequestTest()
        {
            this.request = new ShowRequest();
        }

        [TestCase]
        public void ItShouldHttMethodToGet()
        {
            Assert.That(this.request.Method, Is.EqualTo(HttpMethod.Get));
        }

        [TestCase]
        public void ItShouldPathsWithExpect()
        {
            Assert.That(Path.Combine(this.request.Paths), Is.EqualTo("session"));
        }

        [TestCase]
        public void ItShouldNeedAuthentication()
        {
            Assert.That(this.request.NeedAuthentication, Is.EqualTo(true));
        }
    }
}
