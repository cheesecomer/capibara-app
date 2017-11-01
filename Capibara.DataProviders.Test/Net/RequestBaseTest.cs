using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;

using Capibara.Net;

namespace Capibara.Test.Net.RequestBaseTest.ExecuteTest
{
    public class GetRequest : RequestBase<object>
    {
        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override string[] Paths { get; } = new string[] { "rooms" };
    }

    public class GetWithAuthenticationRequest : RequestBase<object>
    {
        public override HttpMethod Method { get; } = HttpMethod.Get;

        public override string[] Paths { get; } = new string[] { "rooms" };

        public override bool NeedAuthentication { get; } = true;
    }

    [TestFixture]
    public class WhenGetSuccess : WhenSuccessBase<object>
    {
        protected override RequestBase<object> Request
            => new GetRequest();

        [TestCase]
        public void ItShouldRequestToExpectedUrl()
        {
            Assert.That(this.RequestMessage.RequestUri.AbsoluteUri, Is.EqualTo("http://localhost:3000/api/rooms"));
        }
    }

    [TestFixture]
    public class WhenGetWithAuthenticationSuccess : WhenSuccessBase<object>
    {
        protected override RequestBase<object> Request
            => new GetWithAuthenticationRequest();

        protected override string AccessToken { get; } = "1:bGbDyyVxbSQorRhgyt6R";

        [TestCase]
        public void ItShouldRequestToExpectedUrl()
        {
            Assert.That(this.RequestMessage.RequestUri.AbsoluteUri, Is.EqualTo("http://localhost:3000/api/rooms"));
        }

        [TestCase]
        public void ItShouldAuthenticationHeaderWithExpectedScheme()
        {
            Assert.That(this.RequestMessage.Headers.Authorization?.Scheme, Is.EqualTo("Token"));
        }

        [TestCase]
        public void ItShouldAuthenticationHeaderWithExpectedValue()
        {
            Assert.That(this.RequestMessage.Headers.Authorization?.Parameter, Is.EqualTo("1:bGbDyyVxbSQorRhgyt6R"));
        }
    }

    public class WhenHasntPlatformInitializer
    {
        private object result;

        [SetUp]
        public void SetUp()
        {
            // Environment のセットアップ
            var environment = new Mock<IEnvironment>();
            environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

            // RestClient のセットアップ
            var restClient = new Mock<IRestClient>();
            restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
            restClient
                .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(new HttpResponseMessage());

            // ISecureIsolatedStorage のセットアップ
            var isolatedStorage = new Mock<IIsolatedStorage>();
            isolatedStorage.SetupAllProperties();

            var application = new Mock<ICapibaraApplication>();
            application.SetupGet(x => x.HasPlatformInitializer).Returns(false);

            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            container.RegisterInstance<IEnvironment>(environment.Object);
            container.RegisterInstance<IRestClient>(restClient.Object);
            container.RegisterInstance<IIsolatedStorage>(isolatedStorage.Object);
            container.RegisterInstance<ICapibaraApplication>(application.Object);

            // RequestBase のセットアップ
            var task = new GetRequest().BuildUp(container).Execute();
            task.Wait();
            this.result = task.Result;
        }

        [TestCase]
        public void ItShouldThrowException()
        {
            Assert.That(this.result, Is.Null);
        }
    }

    [TestFixture(HttpStatusCode.NotFound, "{\"message\": \"Foo\"}", typeof(HttpNotFoundException), "Foo")]
    [TestFixture(HttpStatusCode.Unauthorized, "{\"message\": \"Bar\"}", typeof(HttpUnauthorizedException), "Bar")]
    public class WhenGetFail
    {
        private HttpStatusCode httpStatus;

        private string response;

        private string message;

        private Type exceptionClass;

        private GetRequest request;

        public WhenGetFail(HttpStatusCode httpStatus, string response, Type exceptionClass, string message)
        {
            this.httpStatus = httpStatus;
            this.response = response;
            this.exceptionClass = exceptionClass;
            this.message = message;
        }

        [SetUp]
        public void SetUp()
        {
            // Environment のセットアップ
            var environment = new Mock<IEnvironment>();
            environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

            var responseMessage =
                new HttpResponseMessage()
                {
                    StatusCode = httpStatus,
                    Content = new HttpContentHandler()
                    {
                        ResultOfString = this.response
                    }
                };

            // RestClient のセットアップ
            var restClient = new Mock<IRestClient>();
            restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
            restClient
                .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .ReturnsAsync(responseMessage);

            // ISecureIsolatedStorage のセットアップ
            var isolatedStorage = new Mock<IIsolatedStorage>();
            isolatedStorage.SetupAllProperties();

            var application = new Mock<ICapibaraApplication>();
            application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            container.RegisterInstance<IEnvironment>(environment.Object);
            container.RegisterInstance<IRestClient>(restClient.Object);
            container.RegisterInstance<IIsolatedStorage>(isolatedStorage.Object);
            container.RegisterInstance<ICapibaraApplication>(application.Object);

            // RequestBase のセットアップ
            this.request = new GetRequest().BuildUp(container);
        }

        [TestCase]
        public void ItShouldThrowException()
        {
            Assert.ThrowsAsync(exceptionClass, this.request.Execute);
        }

        [TestCase]
        public void ItShouldExtensionMessageWithExpected()
        {
            Assert.That(
                () => this.request.Execute(),
                Throws.TypeOf(exceptionClass).With.Message.EqualTo(this.response));
        }

        [TestCase]
        public void ItShouldErrorMessageWithExpected()
        {
            Assert.That(
                () => this.request.Execute(),
                Throws.TypeOf(exceptionClass).With.Property("Detail").Property("Message").EqualTo(this.message));
        }
    }
}
