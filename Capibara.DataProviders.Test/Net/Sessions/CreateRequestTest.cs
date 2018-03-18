using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

using Capibara.Net;
using Capibara.Net.Sessions;

using NUnit.Framework;
using Moq;

namespace Capibara.Test.Net.Sessions.CreateRequestTest
{
    namespace ExecuteTest
    {
        [TestFixture]
        public class WhenUsingEmail : TestFixtureBase
        {
            private CreateRequest Actual { get; set; }
            
            [SetUp]
            public void SetUp()
            {
                this.Actual = new CreateRequest()
                {
                    Email = "user@email.com",
                    Password = "p@ssword"
                };
            }
            
            [TestCase]
            public void ItShouldRequestWithHttpMethodPost()
            {
                Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Post));
            }
            
            [TestCase]
            public void ItShouldPathsWithExpect()
            {
                Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "session" }));
            }
            
            [TestCase]
            public void ItShouldStringContentWithExpected()
            {
                var expected = "{\"email\":\"user@email.com\",\"password\":\"p@ssword\"}".ToSlim();
                Assert.That(Actual.StringContent.ToSlim(), Is.EqualTo(expected));
            }
        }

        [TestFixture]
        public class WhenUsingOAuth : TestFixtureBase
        {
            private CreateRequest Actual { get; set; }

            [SetUp]
            public void SetUp()
            {
                this.Actual = new CreateRequest()
                {
                    Provider = "twitter",
                    AccessToken = "AccessToken",
                    AccessTokenSecret = "AccessTokenSecret"
                };
            }

            [TestCase]
            public void ItShouldRequestWithHttpMethodPost()
            {
                Assert.That(this.Actual.Method, Is.EqualTo(HttpMethod.Post));
            }

            [TestCase]
            public void ItShouldPathsWithExpect()
            {
                Assert.That(this.Actual.Paths, Is.EqualTo(new[] { "session" }));
            }

            [TestCase]
            public void ItShouldStringContentWithExpected()
            {
                var expected = "{\"provider\":\"twitter\",\"access_token\":\"AccessToken\",\"access_token_secret\":\"AccessTokenSecret\"}".ToSlim();
                Assert.That(Actual.StringContent.ToSlim(), Is.EqualTo(expected));
            }
        }
    }
}
