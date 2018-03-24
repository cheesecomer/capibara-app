using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.IO;

using Moq;
using Unity;
using NUnit.Framework;

using Capibara.Net;

namespace Capibara.Test.Net
{
    namespace RequestBaseNonGenericTest.ExecuteTest
    {
        public class GetRequest : RequestBase
        {
            public override HttpMethod Method { get; } = HttpMethod.Get;

            public override string[] Paths { get; } = new string[] { "rooms" };
        }

        public class GetWithAuthenticationRequest : RequestBase
        {
            public override HttpMethod Method { get; } = HttpMethod.Get;

            public override string[] Paths { get; } = new string[] { "rooms" };

            public override bool NeedAuthentication { get; } = true;
        }

        [TestFixture]
        public class WhenGetSuccess : TestFixtureBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                new GetRequest().BuildUp(this.Container).Execute().Wait();
            }

            [TestCase]
            public void ItShouldRequestToExpectedUrl()
            {
                Assert.That(this.RequestMessage.RequestUri.AbsoluteUri, Is.EqualTo("http://localhost:3000/api/rooms"));
            }
        }

        [TestFixture]
        public class WhenGetWithAuthenticationSuccess : TestFixtureBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.IsolatedStorage.AccessToken = "1:bGbDyyVxbSQorRhgyt6R";

                new GetWithAuthenticationRequest().BuildUp(this.Container).Execute().Wait();
            }

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

        public class WhenHasntPlatformInitializer : TestFixtureBase
        {
            private bool result;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var application = new Mock<ICapibaraApplication>();
                application.SetupGet(x => x.HasPlatformInitializer).Returns(false);

                var container = this.Container;
                container.RegisterInstance<ICapibaraApplication>(application.Object);

                // RequestBase のセットアップ
                var task = new GetRequest().BuildUp(container).Execute();
                task.Wait();
                this.result = task.IsCompleted;
            }

            [TestCase]
            public void ItShouldThrowException()
            {
                Assert.That(this.result, Is.True);
            }
        }

        [TestFixture(HttpStatusCode.NotFound, "{\"message\": \"Foo\"}", typeof(HttpNotFoundException), "Foo")]
        [TestFixture(HttpStatusCode.Unauthorized, "{\"message\": \"Bar\"}", typeof(HttpUnauthorizedException), "Bar")]
        public class WhenGetFail : TestFixtureBase
        {
            protected override HttpStatusCode HttpStabStatusCode => httpStatus;

            protected override string HttpStabResponse => response;

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
            public override void SetUp()
            {
                base.SetUp();

                // RequestBase のセットアップ
                this.request = new GetRequest().BuildUp(this.Container);
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

    namespace RequestBaseTest.ExecuteTest
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

        public class PostWithAuthenticationRequest : RequestBase<object>
        {
            public override HttpMethod Method { get; } = HttpMethod.Post;

            public override string[] Paths { get; } = new string[] { "sessions" };

            public override bool NeedAuthentication { get; } = true;

            public override string StringContent => "{ \"email\": \"example@email.com\" }";

            public override string ContentType => "application/json";
        }
        
        [TestFixture]
        public class WhenGetSuccess : TestFixtureBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                new GetRequest().BuildUp(this.Container).Execute().Wait();
            }
            
            [TestCase]
            public void ItShouldRequestToExpectedUrl()
            {
                Assert.That(this.RequestMessage.RequestUri.AbsoluteUri, Is.EqualTo("http://localhost:3000/api/rooms"));
            }
        }
        
        [TestFixture]
        public class WhenGetWithAuthenticationSuccess : TestFixtureBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.IsolatedStorage.AccessToken = "1:bGbDyyVxbSQorRhgyt6R";
                
                new GetWithAuthenticationRequest().BuildUp(this.Container).Execute().Wait();
            }
            
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

        [TestFixture]
        public class WhenPostWithAuthenticationSuccess : TestFixtureBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.IsolatedStorage.AccessToken = "1:bGbDyyVxbSQorRhgyt6R";

                new PostWithAuthenticationRequest().BuildUp(this.Container).Execute().Wait();
            }

            [TestCase]
            public void ItShouldRequestToExpectedUrl()
            {
                Assert.That(this.RequestMessage.RequestUri.AbsoluteUri, Is.EqualTo("http://localhost:3000/api/sessions"));
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
        
        public class WhenHasntPlatformInitializer : TestFixtureBase
        {
            private object result;
            
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var application = new Mock<ICapibaraApplication>();
                application.SetupGet(x => x.HasPlatformInitializer).Returns(false);
                this.Container.RegisterInstance<ICapibaraApplication>(application.Object);
                
                // RequestBase のセットアップ
                var task = new GetRequest().BuildUp(this.Container).Execute();
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
        public class WhenGetFail : TestFixtureBase
        {
            protected override HttpStatusCode HttpStabStatusCode => httpStatus;
            
            protected override string HttpStabResponse => response;
            
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
            public override void SetUp()
            {
                base.SetUp();

                // RequestBase のセットアップ
                this.request = new GetRequest().BuildUp(this.Container);
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
}