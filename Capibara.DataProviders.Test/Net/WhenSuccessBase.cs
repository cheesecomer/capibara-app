using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

using Capibara.Net;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;

namespace Capibara.Test.Net
{
    [TestFixture]
    public abstract class WhenSuccessBase<TResponse> where TResponse : class, new()
    {
        protected HttpRequestMessage RequestMessage;

        protected virtual string ResultOfString => string.Empty;
        
        protected virtual string AccessToken => string.Empty;

        protected abstract RequestBase<TResponse> Request { get; }

        [SetUp]
        public void Setup()
        {
            // Environment のセットアップ
            var environment = new Mock<IEnvironment>();
            environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/api/");

            var responseMessage =
                new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new HttpContentHandler()
                    {
                        ResultOfString = this.ResultOfString
                    }
                };

            // RestClient のセットアップ
            var restClient = new Mock<IRestClient>();
            restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
            restClient
                .Setup(x => x.GenerateAuthenticationHeader(It.Is<string>(v => v == this.AccessToken)))
                .Returns(new AuthenticationHeaderValue("Token", this.AccessToken));
            restClient
                .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                .Callback<HttpRequestMessage>(x => this.RequestMessage = x)
                .ReturnsAsync(responseMessage);

            // ISecureIsolatedStorage のセットアップ
            var isolatedStorage = new Mock<IIsolatedStorage>();
            isolatedStorage.SetupAllProperties();
            isolatedStorage.SetupGet(x => x.AccessToken).Returns(this.AccessToken);

            var application = new Mock<ICapibaraApplication>();
            application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

            var container = new UnityContainer();
            container.RegisterInstance<IUnityContainer>(container);
            container.RegisterInstance<IEnvironment>(environment.Object);
            container.RegisterInstance<IRestClient>(restClient.Object);
            container.RegisterInstance<IIsolatedStorage>(isolatedStorage.Object);
            container.RegisterInstance<ICapibaraApplication>(application.Object);

            Request.BuildUp(container).Execute().Wait();
        }
    }
}
