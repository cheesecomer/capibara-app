using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;

using Capibara.Models;
using Capibara.Net;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Capibara.Test.Models.UserTest
{
    namespace DeserializeTest
    {
        [TestFixture]
        public class WhenSuccess
        {
            private User actual;

            [SetUp]
            public void Setup()
            {
                var json = "{ \"id\": 99999, \"nickname\": \"FooBar. Yes!Yes!Yeeeeees!\"}";
                this.actual = JsonConvert.DeserializeObject<User>(json);
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.actual.Nickname, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.actual.Id, Is.EqualTo(99999));
            }
        }
    }

    namespace SignUpTest
    {
        [TestFixture]
        public class WhenSuccess
        {
            bool isSucceed;

            bool isFailed;

            protected User model;

            [SetUp]
            public void Setup()
            {
                // Environment のセットアップ
                var environment = new Mock<IEnvironment>();
                environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

                var responseMessage =
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new Net.HttpContentHandler()
                        {
                            ResultOfString = "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\", \"user_id\": 999}"
                        }
                    };

                // RestClient のセットアップ
                var restClient = new Mock<IRestClient>();
                restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(responseMessage);

                // ISecureIsolatedStorage のセットアップ
                var secureIsolatedStorage = new Mock<ISecureIsolatedStorage>();
                secureIsolatedStorage.SetupAllProperties();
                secureIsolatedStorage.Setup(x => x.Save());

                var application = new Mock<ICapibaraApplication>();
                application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

                var container = new UnityContainer();
                container.RegisterInstance<IUnityContainer>(container);
                container.RegisterInstance<IEnvironment>(environment.Object);
                container.RegisterInstance<IRestClient>(restClient.Object);
                container.RegisterInstance<ISecureIsolatedStorage>(secureIsolatedStorage.Object);
                container.RegisterInstance<ICapibaraApplication>(application.Object);

                this.model = new User() { Nickname = "xxxxx" }.BuildUp(container);

                this.model.SignUpFail += (sender, e) => this.isFailed = true;
                this.model.SignUpSuccess += (sender, e) => this.isSucceed = true;

                this.model.SignUp().Wait();
            }

            [TestCase]
            public void IsShouldSaveTokenInSecureStorage()
            {
                Assert.That(this.model.SecureIsolatedStorage.AccessToken, Is.EqualTo("1:bGbDyyVxbSQorRhgyt6R"));
            }

            [TestCase]
            public void IsShouldSaveEmailInSecureStorage()
            {
                Assert.That(this.model.SecureIsolatedStorage.Email, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldSaveUserIdInSecureStorage()
            {
                Assert.That(this.model.SecureIsolatedStorage.UserId, Is.EqualTo(999));
            }

            [TestCase]
            public void ItShouldNotBeError()
            {
                Assert.That(this.model.Error, Is.Null);
            }

            [TestCase]
            public void ItShouldSignInSuccessEventToOccur()
            {
                Assert.That(this.isSucceed, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSignInFailEventToNotOccur()
            {
                Assert.That(this.isFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFail
        {
            protected User model;

            bool isSucceed;

            bool isFailed;

            [SetUp]
            public void Setup()
            {
                // Environment のセットアップ
                var environment = new Mock<IEnvironment>();
                environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

                var responseMessage =
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new Net.HttpContentHandler()
                        {
                            ResultOfString = "{ \"message\": \"booo...\"}"
                        }
                    };

                // RestClient のセットアップ
                var restClient = new Mock<IRestClient>();
                restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(responseMessage);

                // ISecureIsolatedStorage のセットアップ
                var secureIsolatedStorage = new Mock<ISecureIsolatedStorage>();
                secureIsolatedStorage.SetupAllProperties();

                var application = new Mock<ICapibaraApplication>();
                application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

                var container = new UnityContainer();
                container.RegisterInstance<IUnityContainer>(container);
                container.RegisterInstance<IEnvironment>(environment.Object);
                container.RegisterInstance<IRestClient>(restClient.Object);
                container.RegisterInstance<ISecureIsolatedStorage>(secureIsolatedStorage.Object);
                container.RegisterInstance<ICapibaraApplication>(application.Object);

                this.model = new User() { Nickname = "xxxxx" }.BuildUp(container);

                this.model.SignUpFail += (sender, e) => this.isFailed = true;
                this.model.SignUpSuccess += (sender, e) => this.isSucceed = true;

                this.model.SignUp().Wait();
            }

            [TestCase]
            public void IsShouldDontSaveTokenInSecureStorage()
            {
                Assert.That(this.model.SecureIsolatedStorage.AccessToken, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldDontSaveEmailInSecureStorage()
            {
                Assert.That(this.model.SecureIsolatedStorage.Email, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldBeError()
            {
                Assert.That(this.model.Error, Is.Not.Null);
            }

            [TestCase]
            public void ItShouldSignInSuccessEventToNotOccur()
            {
                Assert.That(this.isSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSignInFailEventToOccur()
            {
                Assert.That(this.isFailed, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccessWithoutEventHandler
        {
            protected User model;

            [SetUp]
            public void Setup()
            {
                // Environment のセットアップ
                var environment = new Mock<IEnvironment>();
                environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

                var responseMessage =
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.OK,
                        Content = new Net.HttpContentHandler()
                        {
                            ResultOfString = "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\"}"
                        }
                    };

                // RestClient のセットアップ
                var restClient = new Mock<IRestClient>();
                restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(responseMessage);

                // ISecureIsolatedStorage のセットアップ
                var secureIsolatedStorage = new Mock<ISecureIsolatedStorage>();
                secureIsolatedStorage.SetupAllProperties();
                secureIsolatedStorage.Setup(x => x.Save());

                var application = new Mock<ICapibaraApplication>();
                application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

                var container = new UnityContainer();
                container.RegisterInstance<IUnityContainer>(container);
                container.RegisterInstance<IEnvironment>(environment.Object);
                container.RegisterInstance<IRestClient>(restClient.Object);
                container.RegisterInstance<ISecureIsolatedStorage>(secureIsolatedStorage.Object);
                container.RegisterInstance<ICapibaraApplication>(application.Object);

                this.model = new User() { Nickname = "xxxxx" }.BuildUp(container);
            }

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.model.SignUp);
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler
        {
            protected User model;

            bool isSucceed;

            bool isFailed;

            [SetUp]
            public void Setup()
            {
                // Environment のセットアップ
                var environment = new Mock<IEnvironment>();
                environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

                var responseMessage =
                    new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.Unauthorized,
                        Content = new Net.HttpContentHandler()
                        {
                            ResultOfString = "{ \"message\": \"booo...\"}"
                        }
                    };

                // RestClient のセットアップ
                var restClient = new Mock<IRestClient>();
                restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ReturnsAsync(responseMessage);

                // ISecureIsolatedStorage のセットアップ
                var secureIsolatedStorage = new Mock<ISecureIsolatedStorage>();
                secureIsolatedStorage.SetupAllProperties();

                var application = new Mock<ICapibaraApplication>();
                application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

                var container = new UnityContainer();
                container.RegisterInstance<IUnityContainer>(container);
                container.RegisterInstance<IEnvironment>(environment.Object);
                container.RegisterInstance<IRestClient>(restClient.Object);
                container.RegisterInstance<ISecureIsolatedStorage>(secureIsolatedStorage.Object);
                container.RegisterInstance<ICapibaraApplication>(application.Object);

                this.model = new User() { Nickname = "xxxxx" }.BuildUp(container);
            }

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.model.SignUp);
            }
        }

        [TestFixture]
        public class WhenTimeout
        {
            protected User model;

            bool isSucceed;

            bool isFailed;

            [SetUp]
            public void Setup()
            {
                // Environment のセットアップ
                var environment = new Mock<IEnvironment>();
                environment.SetupGet(x => x.ApiBaseUrl).Returns("http://localhost:3000/");

                // RestClient のセットアップ
                var restClient = new Mock<IRestClient>();
                restClient.Setup(x => x.ApplyRequestHeader(It.IsAny<HttpRequestMessage>()));
                restClient
                    .Setup(x => x.SendAsync(It.IsAny<HttpRequestMessage>()))
                    .ThrowsAsync(new WebException());

                // ISecureIsolatedStorage のセットアップ
                var secureIsolatedStorage = new Mock<ISecureIsolatedStorage>();
                secureIsolatedStorage.SetupAllProperties();

                var application = new Mock<ICapibaraApplication>();
                application.SetupGet(x => x.HasPlatformInitializer).Returns(true);

                var container = new UnityContainer();
                container.RegisterInstance<IUnityContainer>(container);
                container.RegisterInstance<IEnvironment>(environment.Object);
                container.RegisterInstance<IRestClient>(restClient.Object);
                container.RegisterInstance<ISecureIsolatedStorage>(secureIsolatedStorage.Object);
                container.RegisterInstance<ICapibaraApplication>(application.Object);

                this.model = new User() { Nickname = "xxxxx" }.BuildUp(container);

                this.model.SignUpFail += (sender, e) => this.isFailed = true;
                this.model.SignUpSuccess += (sender, e) => this.isSucceed = true;

                this.model.SignUp().Wait();
            }

            [TestCase]
            public void IsShouldDontSaveTokenInSecureStorage()
            {
                Assert.That(this.model.SecureIsolatedStorage.AccessToken, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldDontSaveEmailInSecureStorage()
            {
                Assert.That(this.model.SecureIsolatedStorage.Email, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldBeError()
            {
                Assert.That(this.model.Error, Is.Null);
            }

            [TestCase]
            public void ItShouldSignInSuccessEventToNotOccur()
            {
                Assert.That(this.isSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSignInFailEventToOccur()
            {
                Assert.That(this.isFailed, Is.EqualTo(true));
            }
        }
    }
}
