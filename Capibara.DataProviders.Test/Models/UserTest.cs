using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.Net;
using BlockIndexResponse = Capibara.Net.Blocks.IndexResponse;
using SessionCreateResponse = Capibara.Net.Sessions.CreateResponse;

using Moq;
using Unity;
using NUnit.Framework;
using Newtonsoft.Json;
using Capibara.Net.Blocks;

namespace Capibara.Test.Models.UserTest
{
    namespace DeserializeTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected User Subject { get; private set; }

            protected abstract int LoginUserId { get; }

            [SetUp]
            public void Setup()
            {
                var json = "{ \"id\": 99999, \"nickname\": \"FooBar. Yes!Yes!Yeeeeees!\", \"icon_url\": \"http://xxxxxx.com/xxxx.png\", \"is_block\": \"true\", \"accepted\": \"true\" }";
                this.Subject = JsonConvert.DeserializeObject<User>(json).BuildUp(this.GenerateUnityContainer());
                this.IsolatedStorage.UserId = this.LoginUserId;
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.Subject.Nickname, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldIconUrlWithExpected()
            {
                Assert.That(this.Subject.IconUrl, Is.EqualTo("http://xxxxxx.com/xxxx.png"));
            }

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Subject.IsBlock, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldIsAcceptedWithExpected()
            {
                Assert.That(this.Subject.IsAccepted, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenIsOwn : TestBase
        {
            protected override int LoginUserId => 99999;

            [TestCase]
            public void ItShouldIsOwn()
            {
                Assert.That(this.Subject.IsOwn, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenIsOther : TestBase
        {
            protected override int LoginUserId => 99998;

            [TestCase]
            public void ItShouldIsNotOwn()
            {
                Assert.That(this.Subject.IsOwn, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class RestoreTest
    {
        private User Subject;

        [SetUp]
        public void Setup()
        {
            this.Subject = new User { Id = 99999 };
            this.Subject.Restore(new User { Nickname = "FooBar. Yes!Yes!Yeeeeees!", Biography = "...", Id = 99999, IconUrl = "...!!!", IsBlock = true, IsAccepted = true });
        }

        [TestCase]
        public void ItShouldNameWithExpected()
        {
            Assert.That(this.Subject.Nickname, Is.EqualTo("FooBar. Yes!Yes!Yeeeeees!"));
        }

        [TestCase]
        public void ItShouldIdWithExpected()
        {
            Assert.That(this.Subject.Id, Is.EqualTo(99999));
        }

        [TestCase]
        public void ItShouldBiographyWithExpected()
        {
            Assert.That(this.Subject.Biography, Is.EqualTo("..."));
        }

        [TestCase]
        public void ItShouldIconUrlWithExpected()
        {
            Assert.That(this.Subject.IconUrl, Is.EqualTo("...!!!"));
        }

        [TestCase]
        public void ItShouldIsBlockWithExpected()
        {
            Assert.That(this.Subject.IsBlock, Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldIsAcceptedWithExpected()
        {
            Assert.That(this.Subject.IsAccepted, Is.EqualTo(true));
        }
    }

    namespace RefreshTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Subject { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected virtual bool IsOWn { get; }

            protected virtual User Response { get; }

            protected virtual Exception Exception { get; }

            protected bool Result { get; private set; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Subject = new User { Id = 1 }.BuildUp(container);

                var request = new Mock<RequestBase<User>>();
                var methodMock = request.Setup(x => x.Execute());

                if (this.Response != null)
                    methodMock.ReturnsAsync(this.Response);
                else if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    throw new ArgumentException();

                this.RequestFactory.Setup(x => x.UsersShowRequest(this.Subject)).Returns(request.Object);

                if (this.NeedEventHandler)
                {
                    this.Subject.RefreshFail += (sender, e) => this.IsFailed = true;
                    this.Subject.RefreshSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.IsOWn)
                {
                    this.IsolatedStorage.UserId = this.Subject.Id;
                }

                this.Result = this.Subject.Refresh().Result;
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedEventHandler => false;

            protected override User Response => new User
            {
                Id = 1,
                Nickname = "xxxxx!",
                Biography = "...",
                IconUrl = "http://xxxxxx.com/xxxx.png",
                IsBlock = true,
                IsAccepted = true
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNicknameWithExpect()
            {
                Assert.That(this.Subject.Nickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void ItShouldBiographyWithExpect()
            {
                Assert.That(this.Subject.Biography, Is.EqualTo("..."));
            }

            [TestCase]
            public void ItShouldUserIdWithExpect()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldIconUrlWithExpected()
            {
                Assert.That(this.Subject.IconUrl, Is.EqualTo("http://xxxxxx.com/xxxx.png"));
            }

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Subject.IsBlock, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldIsAcceptedWithExpected()
            {
                Assert.That(this.Subject.IsAccepted, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenOwn : WhenSuccess
        {
            protected override bool NeedEventHandler => false;

            protected override bool IsOWn => true;

            [TestCase]
            public void ItShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void ItShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Subject.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.Subject));
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
            protected override bool NeedEventHandler => false;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldNicknameWithExpect()
            {
                Assert.That(this.Subject.Nickname, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldBiographyWithExpect()
            {
                Assert.That(this.Subject.Biography, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldUserIdWithExpect()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class WhenFailWithEventHandler : WhenFail
        {
            protected override bool NeedEventHandler => true;

            [TestCase]
            public void ItShouldSignInSuccessEventToOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSignInFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception Exception => new WebException();
        }
    }

    namespace SignUpTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool Result { get; private set; }

            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Subject { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected virtual SessionCreateResponse Response { get; }

            protected virtual Exception Exception { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Subject = new User { Nickname = "xxxxx" }.BuildUp(container);

                var request = new Mock<RequestBase<SessionCreateResponse>>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Response != null)
                    methodMock.ReturnsAsync(this.Response);
                else if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    throw new ArgumentException();

                this.RequestFactory.Setup(x => x.UsersCreateRequest(this.Subject.Nickname)).Returns(request.Object);

                if (this.NeedEventHandler)
                {
                    this.Subject.SignUpFail += (sender, e) => this.IsFailed = true;
                    this.Subject.SignUpSuccess += (sender, e) => this.IsSucceed = true;
                }

                this.Result = this.Subject.SignUp().Result;
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedEventHandler => true;

            protected override SessionCreateResponse Response => new SessionCreateResponse
            {
                AccessToken = "1:bGbDyyVxbSQorRhgyt6R",
                Id = 999,
                Nickname = "Foo.Bar",
                IsAccepted = true
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSaveTokenInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.AccessToken, Is.EqualTo("1:bGbDyyVxbSQorRhgyt6R"));
            }

            [TestCase]
            public void ItShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.EqualTo("Foo.Bar"));
            }

            [TestCase]
            public void ItShouldSaveUserIdInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserId, Is.EqualTo(999));
            }

            [TestCase]
            public void ItShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Subject.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.Subject));
            }

            [TestCase]
            public void ItShouldSignInSuccessEventToOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldIsAcceptedWithExpected()
            {
                Assert.That(this.Subject.IsAccepted, Is.EqualTo(true));
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
            protected override bool NeedEventHandler => true;

            protected override Exception Exception => new WebException();

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDontSaveTokenInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.AccessToken, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldDontRegisterUserInDIContainer()
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
            protected override bool NeedEventHandler => false;

            protected override SessionCreateResponse Response => new SessionCreateResponse
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
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedEventHandler => false;

            protected override Exception Exception => new WebException();

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
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
            protected bool Result { get; private set; }

            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Subject { get; private set; }

            protected User CurrentUser { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected abstract bool NeedCurrentUserRegistration { get; }

            protected virtual User Response { get; }

            protected virtual Exception Exception { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Subject = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                var request = new Mock<RequestBase<User>>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Response != null)
                    methodMock.ReturnsAsync(this.Response);
                else if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    throw new ArgumentException();

                this.RequestFactory.Setup(x => x.UsersUpdateRequest(this.Subject)).Returns(request.Object);

                if (this.NeedEventHandler)
                {
                    this.Subject.CommitFail += (sender, e) => this.IsFailed = true;
                    this.Subject.CommitSuccess += (sender, e) => this.IsSucceed = true;
                }

                if (this.NeedCurrentUserRegistration)
                {
                    container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this.CurrentUser = new User());
                }

                this.Result = this.Subject.Commit().Result;
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedEventHandler => true;

            protected override bool NeedCurrentUserRegistration => true;

            protected override User Response => new User
            {
                Id = 1,
                Nickname = "xxxxx!",
                Biography = "...",
                IconUrl = "http://xxxxxx.com/xxxx.png"
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.Subject.Nickname, Is.EqualTo("xxxxx!"));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldBiographyWithExpected()
            {
                Assert.That(this.Subject.Biography, Is.EqualTo("..."));
            }

            [TestCase]
            public void ItShouldIconUrlWithExpected()
            {
                Assert.That(this.Subject.IconUrl, Is.EqualTo("http://xxxxxx.com/xxxx.png"));
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
            public void ItShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Subject.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.CurrentUser));
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
            public void ItShouldRegisterUserInDIContainer()
            {
                Assert.That(this.Subject.Container.Resolve(typeof(User), "CurrentUser"), Is.EqualTo(this.Subject));
            }
        }

        [TestFixture]
        public class WhenFail : TestBase
        {
            protected override bool NeedEventHandler => true;

            protected override bool NeedCurrentUserRegistration => false;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDontSaveTokenInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.AccessToken, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldDontSaveUserNicknameInStorage()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldDontRegisterUserInDIContainer()
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
            protected override bool NeedEventHandler => false;

            protected override bool NeedCurrentUserRegistration => true;

            protected override User Response => new User
            {
                Id = 1,
                Nickname = "xxxxx!",
                Biography = "...",
                IconUrl = "http://xxxxxx.com/xxxx.png"
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedEventHandler => false;

            protected override bool NeedCurrentUserRegistration => true;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception Exception => new WebException();
        }
    }

    namespace BlockTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool Result { get; private set; }

            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Subject { get; private set; }

            protected User CurrentUser { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected virtual BlockIndexResponse Response { get; }

            protected virtual Exception Exception { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Subject = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                var request = new Mock<RequestBase<BlockIndexResponse>>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Response != null)
                    methodMock.ReturnsAsync(this.Response);
                else if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    throw new ArgumentException();

                this.RequestFactory.Setup(x => x.BlocksCreateRequest(this.Subject)).Returns(request.Object);

                if (this.NeedEventHandler)
                {
                    this.Subject.BlockFail += (sender, e) => this.IsFailed = true;
                    this.Subject.BlockSuccess += (sender, e) => this.IsSucceed = true;
                }

                this.Result = this.Subject.Block().Result;
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedEventHandler => true;

            protected override BlockIndexResponse Response => new BlockIndexResponse();

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Subject.IsBlock, Is.EqualTo(true));
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
            protected override bool NeedEventHandler => true;

            protected override Exception Exception =>
            new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Subject.IsBlock, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
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
            protected override bool NeedEventHandler => false;

            protected override BlockIndexResponse Response => new BlockIndexResponse();

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Subject.IsBlock, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedEventHandler => false;

            protected override Exception Exception =>
            new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldIsBlockWithExpected()
            {
                Assert.That(this.Subject.IsBlock, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception Exception => new WebException();
        }
    }

    namespace DestroytTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool Result { get; private set; }

            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Subject { get; private set; }

            protected User CurrentUser { get; private set; }

            protected override string HttpStabResponse => string.Empty;

            protected abstract bool NeedEventHandler { get; }

            protected virtual Exception Exception { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Subject = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                this.IsolatedStorage.AccessToken = "AccessToken";
                this.IsolatedStorage.UserId = 1000;
                this.IsolatedStorage.UserNickname = "UserName";
                this.IsolatedStorage.Save();

                var request = new Mock<RequestBase>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    methodMock.Returns(Task.CompletedTask);

                this.RequestFactory.Setup(x => x.UsersDestroyRequest()).Returns(request.Object);

                if (this.NeedEventHandler)
                {
                    this.Subject.DestroyFail += (sender, e) => this.IsFailed = true;
                    this.Subject.DestroySuccess += (sender, e) => this.IsSucceed = true;
                }

                this.Result = this.Subject.Destroy().Result;
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedEventHandler => true;

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldAccessTokenNull()
            {
                Assert.That(this.Subject.IsolatedStorage.AccessToken, Is.Null);
            }

            [TestCase]
            public void ItShouldUserNicknameNull()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.Null);
            }

            [TestCase]
            public void ItShouldUserIdIsZero()
            {
                Assert.That(this.Subject.IsolatedStorage.UserId, Is.Null.Or.EqualTo(0));
            }
        }

        [TestFixture]
        public class WhenFail : TestBase
        {
            protected override bool NeedEventHandler => true;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldAccessTokenPresent()
            {
                Assert.That(this.Subject.IsolatedStorage.AccessToken, Is.EqualTo("AccessToken"));
            }

            [TestCase]
            public void ItShouldUserNicknamePresent()
            {
                Assert.That(this.Subject.IsolatedStorage.UserNickname, Is.EqualTo("UserName"));
            }

            [TestCase]
            public void ItShouldUserIdPresent()
            {
                Assert.That(this.Subject.IsolatedStorage.UserId, Is.EqualTo(1000));
            }
        }

        [TestFixture]
        public class WhenSuccessWithoutEventHandler : TestBase
        {
            protected override bool NeedEventHandler => false;

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedEventHandler => false;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception Exception => new WebException();
        }
    }

    namespace ReportTest
    {
        public abstract class TestBase : TestFixtureBase
        {
            protected bool Result { get; private set; }

            protected bool IsSucceed { get; private set; }

            protected bool IsFailed { get; private set; }

            protected User Subject { get; private set; }

            protected User CurrentUser { get; private set; }

            protected abstract bool NeedEventHandler { get; }

            protected virtual Exception Exception { get; }

            [SetUp]
            public void Setup()
            {
                var container = this.GenerateUnityContainer();

                this.Subject = new User { Id = 1, Nickname = "xxxxx" }.BuildUp(container);

                var request = new Mock<RequestBase>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    methodMock.Returns(Task.CompletedTask);

                this.RequestFactory.Setup(x => x.ReportsCreateRequest(this.Subject, ReportReason.Other, "FooBar")).Returns(request.Object);

                if (this.NeedEventHandler)
                {
                    this.Subject.ReportFail += (sender, e) => this.IsFailed = true;
                    this.Subject.ReportSuccess += (sender, e) => this.IsSucceed = true;
                }

                this.Result = this.Subject.Report(ReportReason.Other, "FooBar").Result;
            }
        }

        [TestFixture]
        public class WhenSuccess : TestBase
        {
            protected override bool NeedEventHandler => true;

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
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
            protected override bool NeedEventHandler => true;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
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
            protected override bool NeedEventHandler => false;

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : TestBase
        {
            protected override bool NeedEventHandler => false;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSucceed, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFailed, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenTimeout : WhenFail
        {
            protected override Exception Exception => new WebException();
        }
    }
}
