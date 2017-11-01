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

namespace Capibara.Test.Models.SessionTest
{
    namespace SignInTest
    {
        public class TestBase: TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected Session Actual { get; private set; }

            protected virtual bool WithEventHandler { get; } = true;

            protected virtual bool NeedExecute { get; } = true;

            [SetUp]
            public void Setup()
            {
                this.Actual = new Session() { Email = "user@email.com", Password = "password" }.BuildUp(this.GenerateUnityContainer());

                if (this.WithEventHandler)
                {
                    this.Actual.SignInFail += (sender, e) => this.IsFailed = true;
                    this.Actual.SignInSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedExecute)
                    this.Actual.SignIn().Wait();
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override string HttpStabResponse =>
            "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\", \"user_id\": 999, \"user_nickname\": \"Foo.Bar\"}";

            [TestCase]
            public void IsShouldSaveTokenInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.AccessToken, Is.EqualTo("1:bGbDyyVxbSQorRhgyt6R"));
            }

            [TestCase]
            public void IsShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.UserNickname, Is.EqualTo("Foo.Bar"));
            }

            [TestCase]
            public void IsShouldSaveUserIdInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.UserId, Is.EqualTo(999));
            }

            [TestCase]
            public void IsShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Actual.Container.IsRegistered(typeof(User), "CurrentUser"), Is.EqualTo(true));
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
            protected override Exception RestException => new WebException();

            [TestCase]
            public void IsShouldDontSaveTokenInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.AccessToken, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.UserNickname, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldNotRegisterUserInDIContainer()
            {
                Assert.That(this.Actual.Container.IsRegistered(typeof(User), "CurrentUser"), Is.EqualTo(false));
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
            protected override string HttpStabResponse =>
            "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\", \"user_id\": 999, \"user_nickname\": \"Foo.Bar\"}";

            protected override bool WithEventHandler { get; } = false;

            protected override bool NeedExecute { get; } = false;

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Actual.SignIn);
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            protected override string HttpStabResponse =>
            "{ \"message\": \"booo...\"}";

            protected override bool WithEventHandler { get; } = false;

            protected override bool NeedExecute { get; } = false;

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Actual.SignIn);
            }
        }
    }
}
