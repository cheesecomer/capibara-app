﻿using System;
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
        public abstract class TestBase : TestFixtureBase
        {
            protected User Actual { get; private set; }

            protected abstract int LoginUserId { get; }

            [SetUp]
            public void Setup()
            {
                var json = "{ \"id\": 99999, \"nickname\": \"FooBar. Yes!Yes!Yeeeeees!\"}";
                this.Actual = JsonConvert.DeserializeObject<User>(json).BuildUp(this.GenerateUnityContainer());
                this.IsolatedStorage.UserId = this.LoginUserId;
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.Actual.Nickname, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Actual.Id, Is.EqualTo(99999));
            }
        }

        [TestFixture]
        public class WhenIsOwn : TestBase
        {
            protected override int LoginUserId => 99999;

            [TestCase]
            public void ItShouldIsOwn()
            {
                Assert.That(this.Actual.IsOwn, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenIsOther : TestBase
        {
            protected override int LoginUserId => 99998;

            [TestCase]
            public void ItShouldIsNotOwn()
            {
                Assert.That(this.Actual.IsOwn, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class RestoreTest
    {
        private User Actual;

        [SetUp]
        public void Setup()
        {
            this.Actual = new User { Id = 99999 };
            this.Actual.Restore(new User { Nickname = "FooBar. Yes!Yes!Yeeeeees!", Biography = "..." });
        }

        [TestCase]
        public void ItShouldNameWithExpected()
        {
            Assert.That(this.Actual.Nickname, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
        }

        [TestCase]
        public void ItShouldIdWithExpected()
        {
            Assert.That(this.Actual.Id, Is.EqualTo(99999));
        }

        [TestCase]
        public void ItShouldBiographyWithExpected()
        {
            Assert.That(this.Actual.Biography, Is.EqualTo("..."));
        }
    }

    namespace RefreshTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Actual { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedExecute { get; }

            protected virtual bool IsOWn { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Actual = new User { Id = 1 }.BuildUp(container);

                if (this.NeedEventHandler)
                {
                    this.Actual.RefreshFail += (sender, e) => this.IsFailed = true;
                    this.Actual.RefreshSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.IsOWn)
                {
                    this.IsolatedStorage.UserId = this.Actual.Id;
                }

                if (this.NeedExecute)
                    this.Actual.Refresh().Wait();
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => false;

            protected override string HttpStabResponse
            => "{ \"id\": 999, \"nickname\": \"xxxxx!\", \"biography\":\"...\"}";

            [TestCase]
            public void IsShouldNicknameWithExpect()
            {
                Assert.That(this.Actual.Nickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void IsShouldBiographyWithExpect()
            {
                Assert.That(this.Actual.Biography, Is.EqualTo("..."));
            }

            [TestCase]
            public void IsShouldUserIdWithExpect()
            {
                Assert.That(this.Actual.Id, Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class WhenOwn : WhenSuccess
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => false;

            protected override string HttpStabResponse
                => "{ \"id\": 999, \"nickname\": \"xxxxx!\", \"biography\":\"...\"}";

            protected override bool IsOWn => true;

            [TestCase]
            public void IsShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.UserNickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void IsShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Actual.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.Actual));
            }
        }

        [TestFixture]
        public class WhenSuccessWithEventHandler : WhenSuccess
        {
            protected override bool NeedEventHandler => true;

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
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => false;

            protected override string HttpStabResponse
            => "{ \"message\": \"booo...\"}";

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            [TestCase]
            public void IsShouldNicknameWithExpect()
            {
                Assert.That(this.Actual.Nickname, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldBiographyWithExpect()
            {
                Assert.That(this.Actual.Biography, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void IsShouldUserIdWithExpect()
            {
                Assert.That(this.Actual.Id, Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception RestException => new WebException();
        }
    }

    namespace SignUpTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Actual { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedExecute { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Actual = new User() { Nickname = "xxxxx" }.BuildUp(container);

                if (this.NeedEventHandler)
                {
                    this.Actual.SignUpFail += (sender, e) => this.IsFailed = true;
                    this.Actual.SignUpSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedExecute)
                    this.Actual.SignUp().Wait();
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override string HttpStabResponse
            => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\", \"user_id\": 999}";

            [TestCase]
            public void IsShouldSaveTokenInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.AccessToken, Is.EqualTo("1:bGbDyyVxbSQorRhgyt6R"));
            }

            [TestCase]
            public void IsShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.UserNickname, Is.EqualTo("xxxxx"));
            }

            [TestCase]
            public void IsShouldSaveUserIdInStorage()
            {
                Assert.That(this.Actual.IsolatedStorage.UserId, Is.EqualTo(999));
            }

            [TestCase]
            public void IsShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Actual.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.Actual));
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
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override string HttpStabResponse
            => "{ \"message\": \"booo...\"}";

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

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
            public void IsShouldDontRegisterUserInDIContainer()
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
            protected override bool NeedExecute => false;

            protected override bool NeedEventHandler => false;

            protected override string HttpStabResponse
            => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\", \"user_id\": 999}";

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Actual.SignUp);
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedExecute => false;

            protected override bool NeedEventHandler => false;

            protected override string HttpStabResponse
                => "{ \"message\": \"booo...\"}";

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Actual.SignUp);
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception RestException => new WebException();
        }
    }
}
