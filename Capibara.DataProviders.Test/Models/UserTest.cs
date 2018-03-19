using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Linq;

using Capibara.Models;
using OAuthSession = Capibara.Net.OAuth.Session;
using TokenPair = Capibara.Net.OAuth.TokenPair;

using Moq;
using Unity;
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
                var json = "{ \"id\": 99999, \"nickname\": \"FooBar. Yes!Yes!Yeeeeees!\", \"icon_url\": \"http://xxxxxx.com/xxxx.png\", \"is_block\": \"true\" }";
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

            [TestCase]
            public void ItShouldIconUrlWithExpected()
            {
                Assert.That(this.Actual.IconUrl, Is.EqualTo("http://xxxxxx.com/xxxx.png"));
            }

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Actual.IsBlock, Is.EqualTo(true));
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
            this.Actual.Restore(new User { Nickname = "FooBar. Yes!Yes!Yeeeeees!", Biography = "...", Id = 99999, IconUrl = "...!!!", IsBlock = true });
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

        [TestCase]
        public void ItShouldIconUrlWithExpected()
        {
            Assert.That(this.Actual.IconUrl, Is.EqualTo("...!!!"));
        }

        [TestCase]
        public void ItShouldIsBlockWithExpected()
        {
            Assert.That(this.Actual.IsBlock, Is.EqualTo(true));
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
            => "{ \"id\": 1, \"nickname\": \"xxxxx!\", \"biography\":\"...\", \"icon_url\": \"http://xxxxxx.com/xxxx.png\", \"is_block\": \"true\" }";

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

            [TestCase]
            public void ItShouldIconUrlWithExpected()
            {
                Assert.That(this.Actual.IconUrl, Is.EqualTo("http://xxxxxx.com/xxxx.png"));
            }

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Actual.IsBlock, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenOwn : WhenSuccess
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => false;

            protected override string HttpStabResponse
            => "{ \"id\": 1, \"nickname\": \"xxxxx!\", \"biography\":\"...\", \"icon_url\": \"http://xxxxxx.com/xxxx.png\", \"is_block\": \"true\" }";

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
            => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\", \"id\": 999}";

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

    namespace CommitTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Actual { get; private set; }

            protected User CurrentUser { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedCurrentUserRegistration { get; }

            protected abstract bool NeedExecute { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Actual = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                if (this.NeedEventHandler)
                {
                    this.Actual.CommitFail += (sender, e) => this.IsFailed = true;
                    this.Actual.CommitSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedCurrentUserRegistration)
                {
                    container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this.CurrentUser = new User());
                }

                if (this.NeedExecute)
                    this.Actual.Commit().Wait();
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override bool NeedCurrentUserRegistration => true;

            protected override string HttpStabResponse
                => "{ \"id\": 1, \"nickname\": \"xxxxx!\", \"biography\":\"...\", \"icon_url\": \"http://xxxxxx.com/xxxx.png\" }";

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.Actual.Nickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Actual.Id, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldBiographyWithExpected()
            {
                Assert.That(this.Actual.Biography, Is.EqualTo("..."));
            }

            [TestCase]
            public void ItShouldIconUrlWithExpected()
            {
                Assert.That(this.Actual.IconUrl, Is.EqualTo("http://xxxxxx.com/xxxx.png"));
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
        public class WhenSuccessWithCurrentUser : WhenSuccess
        {
            protected override bool NeedCurrentUserRegistration => true;


            [TestCase]
            public void IsShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Actual.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.CurrentUser));
            }

            [TestCase]
            public void ItShouldCurrentUserNameWithExpected()
            {
                Assert.That(this.CurrentUser.Nickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void ItShouldCurrentUserIdWithExpected()
            {
                Assert.That(this.CurrentUser.Id, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldCurrentUserBiographyWithExpected()
            {
                Assert.That(this.CurrentUser.Biography, Is.EqualTo("..."));
            }

            [TestCase]
            public void ItShouldCurrentUserIconUrlWithExpected()
            {
                Assert.That(this.CurrentUser.IconUrl, Is.EqualTo("http://xxxxxx.com/xxxx.png"));
            }
        }


        [TestFixture]
        public class WhenSuccessWithoutCurrentUser : WhenSuccess
        {
            protected override bool NeedCurrentUserRegistration => false;


            [TestCase]
            public void IsShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Actual.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.Actual));
            }
        }

        [TestFixture]
        public class WhenFail : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override bool NeedCurrentUserRegistration => false;

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

            protected override bool NeedCurrentUserRegistration => true;

            protected override string HttpStabResponse
                => "{ \"id\": 1, \"nickname\": \"xxxxx!\", \"biography\":\"...\", icon_url : \"http://xxxxxx.com/xxxx.png\" }";

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Actual.Commit);
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedExecute => false;

            protected override bool NeedEventHandler => false;

            protected override bool NeedCurrentUserRegistration => true;

            protected override string HttpStabResponse
                => "{ \"message\": \"booo...\"}";

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Actual.Commit);
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception RestException => new WebException();
        }
    }

    namespace BlockTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Actual { get; private set; }

            protected User CurrentUser { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedExecute { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Actual = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                if (this.NeedEventHandler)
                {
                    this.Actual.BlockFail += (sender, e) => this.IsFailed = true;
                    this.Actual.BlockSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedExecute)
                    this.Actual.Block().Wait();
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Actual.IsBlock, Is.EqualTo(true));
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

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Actual.IsBlock, Is.EqualTo(false));
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
                => "{ \"id\": 1, \"nickname\": \"xxxxx!\", \"biography\":\"...\", icon_url : \"http://xxxxxx.com/xxxx.png\" }";

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.DoesNotThrowAsync(this.Actual.Block);
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
                Assert.DoesNotThrowAsync(this.Actual.Block);
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception RestException => new WebException();
        }
    }

    namespace OAuthAuthorizeTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Actual { get; private set; }

            protected User CurrentUser { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedExecute { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.TwitterOAuthService
                    .Setup(x => x.AuthorizeAsync())
                    .ReturnsAsync(this.Authorize);

                this.Actual = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                if (this.NeedEventHandler)
                {
                    this.Actual.OAuthAuthorizeFail += (sender, e) => this.IsFailed = true;
                    this.Actual.OAuthAuthorizeSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedExecute)
                    this.Actual.OAuthAuthorize(OAuthProvider.Twitter).Wait();
            }

            public abstract OAuthSession Authorize();
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedExecute => true;

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

            public override OAuthSession Authorize()
            => new OAuthSession();
        }

        [TestFixture]
        public class WhenFail : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

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

            public override OAuthSession Authorize()
            => throw new NotImplementedException();
        }
    }


    namespace SignUpWithOAuthTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Actual { get; private set; }

            protected User CurrentUser { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedExecute { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.IsolatedStorage.OAuthCallbackUrl = new Uri("capibara://cheese-comer.com/oauth/twitter?oauth_verifier=AbcdeFgh");

                this.TwitterOAuthService
                    .Setup(x => x.GetAccessTokenAsync(It.IsAny<TokenPair>(), It.Is<string>(v => v == "AbcdeFgh")))
                    .ReturnsAsync(this.GetAccessToken);

                this.Actual = new User().BuildUp(container);

                if (this.NeedEventHandler)
                {
                    this.Actual.SignUpFail += (sender, e) => this.IsFailed = true;
                    this.Actual.SignUpSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedExecute)
                    this.Actual.SignUpWithOAuth().Wait();
            }

            public abstract TokenPair GetAccessToken();
        }

        [TestFixture]
        public class WhenGetAccessTokenSuccess : TestBase
        {
            protected override bool NeedExecute => true;

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

            public override TokenPair GetAccessToken()
            => new TokenPair();
        }

        [TestFixture]
        public class WhenGetAccessTokenFail : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

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

            public override TokenPair GetAccessToken()
            => throw new NotImplementedException();
        }

        [TestFixture]
        public class WhenSuccess : WhenGetAccessTokenSuccess
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override string HttpStabResponse
            => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\", \"id\": 999, \"nickname\": \"xxxxx\"}";

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
                Assert.That(this.Actual.Container.Resolve<User>("CurrentUser").Id, Is.EqualTo(this.Actual.Id));
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

            public override TokenPair GetAccessToken()
            => new TokenPair();

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
    }

    namespace DestroytTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Actual { get; private set; }

            protected User CurrentUser { get; private set; }

            protected override string HttpStabResponse => string.Empty;

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedExecute { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Actual = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                this.IsolatedStorage.AccessToken = "AccessToken";
                this.IsolatedStorage.OAuthCallbackUrl = new Uri("foobar://example.com/");
                this.IsolatedStorage.OAuthRequestTokenPair = new TokenPair
                {
                    Token = "Token",
                    TokenSecret = "TokenSecret"
                };
                this.IsolatedStorage.UserId = 1000;
                this.IsolatedStorage.UserNickname = "UserName";
                this.IsolatedStorage.Save();

                if (this.NeedEventHandler)
                {
                    this.Actual.CommitFail += (sender, e) => this.IsFailed = true;
                    this.Actual.CommitSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedExecute)
                    this.Actual.Destroy().Wait();
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            [TestCase]
            public void ItShouldAccessTokenNull()
            {
                Assert.That(this.IsolatedStorage.AccessToken, Is.Null);
            }

            [TestCase]
            public void IsShouldUserNicknameNull()
            {
                Assert.That(this.Actual.IsolatedStorage.UserNickname, Is.Null);
            }

            [TestCase]
            public void IsShouldOAuthCallbackUrlNull()
            {
                Assert.That(this.Actual.IsolatedStorage.OAuthCallbackUrl, Is.Null);
            }

            [TestCase]
            public void IsShouldOAuthRequestTokenPairNull()
            {
                Assert.That(this.Actual.IsolatedStorage.OAuthRequestTokenPair, Is.Null);
            }

            [TestCase]
            public void IsShouldUserIdIsZero()
            {
                Assert.That(this.Actual.IsolatedStorage.UserId, Is.Null.Or.EqualTo(0));
            }
        }

        [TestFixture]
        public class WhenFail : TestBase
        {
            protected override bool NeedExecute => true;

            protected override bool NeedEventHandler => true;

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.NotFound;

            [TestCase]
            public void ItShouldAccessTokenPresent()
            {
                Assert.That(this.IsolatedStorage.AccessToken, Is.EqualTo("AccessToken"));
            }

            [TestCase]
            public void IsShouldUserNicknamePresent()
            {
                Assert.That(this.Actual.IsolatedStorage.UserNickname, Is.EqualTo("UserName"));
            }

            [TestCase]
            public void IsShouldOAuthCallbackUrlPresent()
            {
                Assert.That(this.Actual.IsolatedStorage.OAuthCallbackUrl?.ToString(), Is.EqualTo("foobar://example.com/"));
            }

            [TestCase]
            public void IsShouldOAuthRequestTokenPairTokenPresent()
            {
                Assert.That(this.Actual.IsolatedStorage.OAuthRequestTokenPair?.Token, Is.EqualTo("Token"));
            }

            [TestCase]
            public void IsShouldOAuthRequestTokenPairTokenSecretPresent()
            {
                Assert.That(this.Actual.IsolatedStorage.OAuthRequestTokenPair?.TokenSecret, Is.EqualTo("TokenSecret"));
            }

            [TestCase]
            public void IsShouldUserIdPresent()
            {
                Assert.That(this.Actual.IsolatedStorage.UserId, Is.EqualTo(1000));
            }
        }

        [TestFixture]
        public class WhenSuccessWithoutEventHandler : TestBase
        {
            protected override bool NeedExecute => false;

            protected override bool NeedEventHandler => false;

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.That(this.Actual.Destroy().Result, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedExecute => false;

            protected override bool NeedEventHandler => false;

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            [TestCase]
            public void IsShouldNotException()
            {
                Assert.That(this.Actual.Destroy().Result, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception RestException => new WebException();
        }
    }
}
