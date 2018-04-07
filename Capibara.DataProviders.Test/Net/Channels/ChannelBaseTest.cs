﻿using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;

using Capibara.Net;
using Capibara.Net.Channels;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Capibara.Test.Net.Channels.ChannelBaseTest
{
    public class MockChannelIdentifier : IChannelIdentifier
    {
        [JsonProperty("channel")]
        public string Channel { get; } = "MockChannel";
    }

    public class MockChannel : ChannelBase<object>
    {
        public override IChannelIdentifier ChannelIdentifier { get;  } = new MockChannelIdentifier();
    }

    public abstract class TestFixtureBase : Capibara.Test.TestFixtureBase
    {
        protected ChannelBase<object> Channel { get; } = new MockChannel();

        protected Mock<ChannelCableBase> Cable;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Cable = new Mock<ChannelCableBase>();

            this.ChannelCableFactory.Setup(x => x.Create()).Returns(this.Cable.Object);

            this.Channel.BuildUp(this.Container);
        }
    }

    namespace DisposeTest
    {
        [TestFixture]
        public class WhenConnected : TestFixtureBase
        {
            [TestCase]
            public void ItShouldNotThrow()
            {
                this.Channel.Connect();
                Assert.DoesNotThrow(this.Channel.Dispose);
            }
        }

        [TestFixture]
        public class WhenNotConnect : TestFixtureBase
        {
            [TestCase]
            public void ItShouldNotThrow()
            {
                Assert.DoesNotThrow(this.Channel.Dispose);
            }
        }
    }

    namespace IsOpenTest
    {
        [TestFixture]
        public class WhenIsOpen : TestFixtureBase
        {
            [TestCase]
            public void ItShouldIsOpen()
            {
                this.Channel.Connect();
                this.Cable.SetupGet(x => x.IsOpen).Returns(true);
                Assert.That(this.Channel.IsOpen, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenIsNotOpen : TestFixtureBase
        {
            [TestCase]
            public void ItShouldIsOpen()
            {
                this.Channel.Connect();
                this.Cable.SetupGet(x => x.IsOpen).Returns(false);
                Assert.That(this.Channel.IsOpen, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenNotConnect : TestFixtureBase
        {
            [TestCase]
            public void ItShouldIsOpen()
            {
                Assert.That(this.Channel.IsOpen, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class ConnectTest : TestFixtureBase
    {
        [TestCase]
        public void ItShouldCallConnect()
        {
            var isConnectCalled = false;
            this.Cable.Setup(x => x.Connect()).Callback(() => isConnectCalled = true);

            this.Channel.Connect();
            Assert.That(isConnectCalled, Is.EqualTo(true));
        }
    }
    namespace CloseTest
    {
        [TestFixture]
        public class WhenConnected : TestFixtureBase
        {
            [TestCase]
            public void ItShouldCallConnect()
            {
                this.Channel.Connect();
                Assert.DoesNotThrowAsync(this.Channel.Close);
            }
        }

        [TestFixture]
        public class WhenNotConnected : TestFixtureBase
        {
            [TestCase]
            public void ItShouldCallConnect()
            {
                Assert.DoesNotThrowAsync(this.Channel.Close);
            }
        }
    }

    namespace OnConnectedTest
    {
        public class WhenWihtEventHandler : TestFixtureBase
        {
            bool IsSendSubscribe = false;
            bool IsFireEvent = false;
            public override void SetUp()
            {
                base.SetUp();
                this.Cable
                    .Setup(x => x.SendSubscribe(It.Is<IChannelIdentifier>(v => v.Channel == "MockChannel")))
                    .Callback((IChannelIdentifier x) => this.IsSendSubscribe = true)
                    .Returns(Task.CompletedTask);
                this.Channel.Connected += (s, e) => this.IsFireEvent = true;
                this.Channel.Connect();
                this.Cable.Raise(x => x.Connected += null, EventArgs.Empty);
            }

            [TestCase]
            public void ItShouldSendSubscribe()
            {
                Assert.That(this.IsSendSubscribe, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldFireConnectedEvent()
            {
                Assert.That(this.IsFireEvent, Is.EqualTo(true));
            }
        }

        public class WhenWihtoutEventHandler : TestFixtureBase
        {
            bool IsSendSubscribe = false;
            public override void SetUp()
            {
                base.SetUp();
                this.Cable
                    .Setup(x => x.SendSubscribe(It.Is<IChannelIdentifier>(v => v.Channel == "MockChannel")))
                    .Callback((IChannelIdentifier x) => this.IsSendSubscribe = true)
                    .Returns(Task.CompletedTask);
                this.Channel.Connect();
                this.Cable.Raise(x => x.Connected += null, EventArgs.Empty);
            }

            [TestCase]
            public void ItShouldSendSubscribe()
            {
                Assert.That(this.IsSendSubscribe, Is.EqualTo(true));
            }
        }
    }

    namespace OnDisconnectedTest
    {
        public class WhenWihtEventHandler : TestFixtureBase
        {
            bool IsDisposeCalled = false;
            bool IsFireEvent = false;
            public override void SetUp()
            {
                base.SetUp();
                this.Cable
                    .Setup(x => x.Dispose())
                    .Callback(() => this.IsDisposeCalled = true);
                this.Channel.Disconnected += (s, e) => this.IsFireEvent = true;
                this.Channel.Connect();
                this.Cable.Raise(x => x.Disconnected += null, EventArgs.Empty);
            }

            [TestCase]
            public void ItShouldCloseCalled()
            {
                Assert.That(this.IsDisposeCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldFireConnectedEvent()
            {
                Assert.That(this.IsFireEvent, Is.EqualTo(true));
            }
        }

        public class WhenWihtoutEventHandler : TestFixtureBase
        {
            bool IsDisposeCalled = false;
            public override void SetUp()
            {
                base.SetUp();
                this.Channel.Connect();
                this.Cable
                    .Setup(x => x.Dispose())
                    .Callback(() => this.IsDisposeCalled = true);
                this.Channel.Connect();
                this.Cable.Raise(x => x.Disconnected += null, EventArgs.Empty);
            }

            [TestCase]
            public void ItShouldCloseCalled()
            {
                Assert.That(this.IsDisposeCalled, Is.EqualTo(true));
            }
        }
    }

    namespace OnMessageReceiveTest
    {
        public class WhenWihtEventHandler : TestFixtureBase
        {
            bool IsFireEvent = false;
            public override void SetUp()
            {
                base.SetUp();
                this.Channel.MessageReceive += (s, e) => this.IsFireEvent = true;
                this.Channel.Connect();
                this.Cable.Raise(x => x.MessageReceived += null, new EventArgs<string>("{\"message\": \"welcome\"}"));
            }

            [TestCase]
            public void ItShouldFireConnectedEvent()
            {
                Assert.That(this.IsFireEvent, Is.EqualTo(true));
            }
        }

        public class WhenWihtoutEventHandler : TestFixtureBase
        {
            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                this.Channel.Connect();
                Assert.DoesNotThrow(() => this.Cable.Raise(x => x.MessageReceived += null, new EventArgs<string>("{\"message\": \"welcome\"}")));
            }
        }
    }


    namespace OnRejectSubscriptionReceived
    {
        public class WhenWihtEventHandler : TestFixtureBase
        {
            bool IsFireEvent = false;
            public override void SetUp()
            {
                base.SetUp();
                this.Channel.RejectSubscription += (s, e) => this.IsFireEvent = true;
                this.Channel.Connect();
                this.Cable.Raise(x => x.RejectSubscriptionReceived += null, EventArgs.Empty);
            }

            [TestCase]
            public void ItShouldFireConnectedEvent()
            {
                Assert.That(this.IsFireEvent, Is.EqualTo(true));
            }
        }

        public class WhenWihtoutEventHandler : TestFixtureBase
        {
            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                this.Channel.Connect();
                Assert.DoesNotThrow(() => this.Cable.Raise(x => x.RejectSubscriptionReceived += null, EventArgs.Empty));
            }
        }
    }
}
