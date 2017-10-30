using System.Net.Http;

using Capibara.Net;
using Capibara.Net.Users;

using NUnit.Framework;

using CreateResponse = Capibara.Net.Sessions.CreateResponse;

namespace Capibara.Test.Net.Users.CreateRequestTest.ExecuteTest
{
    [TestFixture]
    public class WhenSuccess : WhenSuccessBase<CreateResponse>
    {
        protected override RequestBase<CreateResponse> Request
            => new CreateRequest { Nickname = "Foo.BAR" };

        protected override string ResultOfString
            => "{ \"id\": 1, \"nickname\": \"Test User\", \"gender\": 0 }";

        [TestCase]
        public void ItShouldRequestWithHttpMethodPost()
        {
            Assert.That(this.RequestMessage.Method, Is.EqualTo(HttpMethod.Post));
        }

        [TestCase]
        public void ItShouldRequestToExpectedUrl()
        {
            Assert.That(this.RequestMessage.RequestUri.AbsoluteUri, Is.EqualTo("http://localhost:3000/api/users"));
        }

        [TestCase]
        public void ItShouldRequestWithExpectedJson()
        {
            var expected = "{\"nickname\":\"Foo.BAR\"}".ToSlim();
            var task = this.RequestMessage.Content?.ReadAsStringAsync();
            task?.Wait();
            Assert.That(task?.Result?.ToSlim(), Is.EqualTo(expected));
        }
    }
}
