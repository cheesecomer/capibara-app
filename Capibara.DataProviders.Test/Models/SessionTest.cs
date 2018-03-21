using System;
using System.Net;

using Capibara.Models;
using Capibara.Net;
using Capibara.Net.Sessions;

using Moq;
using NUnit.Framework;

namespace Capibara.Test.Models.SessionTest
{
    namespace SignInTest
    {
        public class TestBase: TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected Session Subject { get; private set; }

            protected virtual bool WithEventHandler { get; } = true;

            protected virtual CreateResponse Response { get; }

            protected virtual Exception Exception { get; }

            protected bool Result { get; private set; }

            [SetUp]
            public void Setup()
            {
                this.Subject = new Session { Email = "user@email.com", Password = "password" }.BuildUp(this.GenerateUnityContainer());

                var request = new Mock<RequestBase<CreateResponse>>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Response != null)
                    methodMock.ReturnsAsync(this.Response);
                else if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    throw new ArgumentException();

                this.RequestFactory.Setup(x => x.SessionsCreateRequest("user@email.com", "password")).Returns(request.Object);

                if (this.WithEventHandler)
                {
                    this.Subject.SignInFail += (sender, e) => this.IsFailed = true;
                    this.Subject.SignInSuccess += (sender, e) => this.IsSucceed = true;
                }

                this.Result = this.Subject.SignIn().Result;
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override CreateResponse Response => new CreateResponse
            {
                AccessToken = "1:bGbDyyVxbSQorRhgyt6R",
                Id = 999,
                Nickname = "Foo.Bar"
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void IsShouldSaveTokenInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.AccessToken, Is.EqualTo("1:bGbDyyVxbSQorRhgyt6R"));
            }

            [TestCase]
            public void IsShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.EqualTo("Foo.Bar"));
            }

            [TestCase]
            public void IsShouldSaveUserIdInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserId, Is.EqualTo(999));
            }

            [TestCase]
            public void IsShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Subject.Container.IsRegistered(typeof(User), "CurrentUser"), Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSignInSuccessEventToOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSignInFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFail : TestBase
        {
            protected override Exception Exception => new WebException();

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void IsShouldDontSaveTokenInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.AccessToken, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldNotRegisterUserInDIContainer()
            {
                Assert.That(this.Subject.Container.IsRegistered(typeof(User), "CurrentUser"), Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSignInSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSignInFailEventToOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccessWithoutEventHandler : TestBase
        {
            protected override bool WithEventHandler { get; } = false;

            protected override CreateResponse Response => new CreateResponse
            {
                AccessToken = "1:bGbDyyVxbSQorRhgyt6R",
                Id = 999,
                Nickname = "Foo.Bar"
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Subject.SignIn);
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool WithEventHandler { get; } = false;

            protected override Exception Exception => new WebException();

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Subject.SignIn);
            }
        }
    }
}
