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

namespace Capibara.Test.Net.Sessions.CreateRequestTest.ExecuteTest
{
    [TestFixture]
    public class WhenSuccess : WhenSuccessBase<CreateResponse>
    {
        protected override RequestBase<CreateResponse> Request
            => new CreateRequest()
                {
                    Email = "user@email.com",
                    Password = "p@ssword"
                };

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
            Assert.That(this.RequestMessage.RequestUri.AbsoluteUri, Is.EqualTo("http://localhost:3000/api/session"));
        }

        [TestCase]
        public void ItShouldRequestWithExpectedJson()
        {
            var expected = "{\"email\":\"user@email.com\",\"password\":\"p@ssword\"}".ToSlim();
            var task = this.RequestMessage.Content?.ReadAsStringAsync();
            task?.Wait();
            Assert.That(task?.Result?.ToSlim(), Is.EqualTo(expected));
        }
    }
}
