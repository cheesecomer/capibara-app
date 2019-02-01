using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Capibara.Models;
using Capibara.Net;
using Capibara.Net.DirectMessages;
using Capibara.Net.Channels;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Capibara.Test.Models.DirectMessageThreadTest
{
    [TestFixture]
    public class ConnectTest : TestFixtureBase
    {
        private DirectMessageThread Subject;

        private Mock<DirectMessageChannelBase> Channel;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Channel = new Mock<DirectMessageChannelBase>();
            this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

            this.ChannelFactory.Setup(x => x.CreateDirectMessageChannel(It.IsAny<User>())).Returns(this.Channel.Object);

            this.Subject = new DirectMessageThread().BuildUp(this.Container);

            this.Subject.Connect().Wait();
        }

        [TestCase]
        public void ItShouldCreateChantChannelOnece()
        {
            this.Channel.SetupGet(x => x.IsOpen).Returns(true);
            this.Subject.Connect().Wait();
            this.ChannelFactory.Verify(x => x.CreateDirectMessageChannel(It.IsAny<User>()), Times.Once());
        }

        [TestCase]
        public void ItShouldConnectCalled()
        {
            this.Channel.Verify(x => x.Connect(), Times.Once());
        }
    }

    namespace CloseTest
    {
        [TestFixture]
        public class WhenConnected : TestFixtureBase
        {
            private DirectMessageThread Subject;

            private Mock<DirectMessageChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<DirectMessageChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateDirectMessageChannel(It.IsAny<User>())).Returns(this.Channel.Object);

                this.Subject = new DirectMessageThread().BuildUp(this.Container);

                this.Subject.Connect().Wait();
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldCloseCalled()
            {
                this.Channel.Verify(x => x.Close(), Times.Once());
            }

            [TestCase]
            public void ItShouldDisposeCalled()
            {
                this.Channel.Verify(x => x.Dispose(), Times.Once());
            }

            [TestCase]
            public void ItShouldClosed()
            {
                Assert.That(this.Subject.IsConnected, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenNotConnected : TestFixtureBase
        {
            private DirectMessageThread Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new DirectMessageThread().BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.DoesNotThrowAsync(this.Subject.Close);
            }

            [TestCase]
            public void ItShouldClosed()
            {
                Assert.That(this.Subject.IsConnected, Is.EqualTo(false));
            }
        }
    }

    namespace SpeakTest
    {
        [TestFixture]
        public abstract class SpeakTestBase : TestFixtureBase
        {
            protected DirectMessageThread Subject;

            protected bool IsSuccess { get; private set; }

            protected bool IsFail { get; private set; }

            protected virtual bool NeedEventHandler { get; } = true;

            protected virtual Exception Exception { get; } = null;

            private Mock<DirectMessageChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<DirectMessageChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);
                var speak = this.Channel.Setup(x => x.Speak(It.IsAny<string>()));
                if (this.Exception.IsNull())
                {
                    speak.Returns(Task.CompletedTask);
                }
                else
                {
                    speak.ThrowsAsync(this.Exception);
                }

                this.ChannelFactory.Setup(x => x.CreateDirectMessageChannel(It.IsAny<User>())).Returns(this.Channel.Object);

                this.Subject = new DirectMessageThread().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                this.Subject = new DirectMessageThread { User = new User { Id = 1 } }.BuildUp(this.Container);

                if (this.NeedEventHandler)
                {
                    this.Subject.SpeakSuccess += (sender, e) => this.IsSuccess = true;
                    this.Subject.SpeakFail += (sender, e) => this.IsFail = true;
                }

                // 接続の完了を待機
                this.Subject.Connect().Wait();

                this.Subject.Speak("Foo. Bar!").Wait();
            }

            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                Assert.DoesNotThrowAsync(() => this.Subject.Speak("Foo. Bar!"));
            }

            [TestCase]
            public void ItShouldSpeakCalled()
            {
                this.Channel.Verify(x => x.Speak("Foo. Bar!"), Times.Once());
            }
        }

        [TestFixture]
        public class WhenSuccessWithoutEventHandler : SpeakTestBase
        {
            protected override bool NeedEventHandler { get; } = false;
        }

        [TestFixture]
        public class WhenSuccessWithEventHandler : SpeakTestBase
        {
            [TestCase]
            public void ItShouldSuccessEventToOccur()
            {
                Assert.That(this.IsSuccess, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailWithoutEventHandler : SpeakTestBase
        {
            protected override bool NeedEventHandler { get; } = false;

            protected override Exception Exception => new Exception();
        }

        [TestFixture]
        public class WhenFailWithEventHandler : SpeakTestBase
        {
            protected override Exception Exception => new Exception();

            [TestCase]
            public void ItShouldSuccessEventToOccur()
            {
                Assert.That(this.IsSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToOccur()
            {
                Assert.That(this.IsFail, Is.EqualTo(true));
            }
        }
    }

    namespace RefreshTest
    {
        [TestFixture]
        public abstract class RefreshTestBase : TestFixtureBase
        {
            protected DirectMessageThread model;

            protected bool IsRefreshSuccess { get; private set; }

            protected bool IsRefreshFail { get; private set; }

            protected virtual bool NeedEventHandler { get; } = true;

            protected virtual ShowResponse Response { get; }

            protected virtual Exception Exception { get; }

            protected bool Result { get; private set; }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.model = new DirectMessageThread { User = new User { Id = 1 } }.BuildUp(this.Container);

                if (this.NeedEventHandler)
                {
                    this.model.RefreshSuccess += (sender, e) => this.IsRefreshSuccess = true;
                    this.model.RefreshFail += (sender, e) => this.IsRefreshFail = true;
                }

                var request = new Mock<RequestBase<ShowResponse>>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Response != null)
                    methodMock.ReturnsAsync(this.Response);
                else if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    throw new ArgumentException();

                this.RequestFactory.Setup(x => x.DirectMessagesShowRequest(It.IsAny<User>(), 0)).Returns(request.Object);

                // リフレッシュの終了を待機
                this.Result = this.model.Refresh().Result;
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithoutEventHandler : RefreshTestBase
        {
            protected override bool NeedEventHandler { get; } = false;

            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToNotOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenUnauthorized : RefreshTestBase
        {
            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

            [TestCase]
            public void ItShouldFail()
            {
                Assert.That(this.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToNotOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenEmptyWithoutEventHandler : RefreshTestBase
        {
            protected override bool NeedEventHandler { get; } = false;

            protected override ShowResponse Response => new ShowResponse();

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToNotOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenEmpty : RefreshTestBase
        {
            protected override ShowResponse Response => new ShowResponse();

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenHasMessage : RefreshTestBase
        {
            protected override ShowResponse Response
            {
                get
                {
                    var result = new ShowResponse();
                    result.DirectMessages.Add(new DirectMessage { Id = 1 });
                    result.DirectMessages.Add(new DirectMessage { Id = 2 });

                    return result;
                }
            }

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenHasDuplicateMessage : RefreshTestBase
        {
            protected override ShowResponse Response
            {
                get
                {
                    var result = new ShowResponse();
                    result.DirectMessages.Add(new DirectMessage { Id = 9999 });
                    result.DirectMessages.Add(new DirectMessage { Id = 9999 });

                    return result;
                }
            }

            [TestCase]
            public void ItShouldRefreshSuccessEventToOccur()
            {
                Assert.That(this.IsRefreshSuccess, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsRefreshFail, Is.EqualTo(false));
            }
        }
    }

        [TestFixture]
    public class OnMessageReceiveTest : TestFixtureBase
    {
        private DirectMessageThread Subject;

        private Mock<DirectMessageChannelBase> Channel;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Channel = new Mock<DirectMessageChannelBase>();
            this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

            this.ChannelFactory.Setup(x => x.CreateDirectMessageChannel(It.IsAny<User>())).Returns(this.Channel.Object);

            this.Subject = new DirectMessageThread().BuildUp(this.Container);

            this.Subject.Connect().Wait();

            var message = new DirectMessage { Id = 999, Sender = new User(), Content = "FooBar. Yes!Yes!Yeeeeees!", At = DateTimeOffset.Now };

            this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<DirectMessage>(message));
        }

        [TestCase]
        public void ItShouldMessagesCountWithExpected()
        {
            Assert.That(this.Subject.DirectMessages.Count, Is.EqualTo(1));
        }
    }
}
