using System;
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
        protected override IChannelIdentifier ChannelIdentifier { get;  } = new MockChannelIdentifier();
    }

    public abstract class TestFixtureBase : ChannelTestFixtureBase<object>
    {
        protected override IChannel<object> Channel { get; } = new MockChannel();
    }

    namespace DisposeTest
    {
        [TestFixture]
        public class WhenConnected : TestFixtureBase
        {
            protected override bool NeedConnect => true;

            [TestCase]
            public void ItShouldNotThrow()
            {
                Assert.DoesNotThrow(this.Channel.Dispose);
            }
        }

        [TestFixture]
        public class WhenNotConnect : TestFixtureBase
        {
            protected override bool NeedConnect => false;

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
        public class WhenConnected : TestFixtureBase
        {
            protected override bool NeedConnect => true;

            [TestCase]
            public void ItShouldIsOpen()
            {
                Assert.That(this.Channel.IsOpen, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenNotConnect : TestFixtureBase
        {
            protected override bool NeedConnect => false;

            [TestCase]
            public void ItShouldIsNotOpen()
            {
                Assert.That(this.Channel.IsOpen, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class ConnectTest : TestFixtureBase
    {
        protected override bool HasEventHandler => false;

        protected override bool NeedWaitConnect => false;

        [TestCase]
        public void ItShouldCallConnect()
        {
            Assert.That(this.IsWebSocketConnectCalled, Is.EqualTo(true));
        }
    }

    namespace CloseTest
    {

        [TestFixture]
        public class WhenConnected : TestFixtureBase
        {
            protected override bool HasEventHandler => false;

            protected override bool NeedWaitConnect => false;

            [TestCase]
            public void ItShouldCallConnect()
            {
                Assert.DoesNotThrowAsync(this.Channel.Close);
            }
        }

        [TestFixture]
        public class WhenNotConnected
        {

            [TestCase]
            public void ItShouldCallConnect()
            {
                Assert.DoesNotThrowAsync(new MockChannel().Close);
            }
        }
    }

    namespace OnConnectedTest
    {
        public abstract class TestFixtureBase : ChannelBaseTest.TestFixtureBase
        {
            [TestCase]
            public void ItShouldCallSendSubscribe()
            {
                var command = JsonConvert.DeserializeObject<Dictionary<string, string>>(this.SendMessage);
                Assert.That(command.ValueOrDefault("command"), Is.EqualTo("subscribe"));
            }
        }

        public class WhenConnectSuccessEventHandlerExists : TestFixtureBase
        {
            protected override bool HasEventHandler => true;

            [TestCase]
            public void ItShouldConnectedEventToOccur()
            {
                Assert.That(this.IsFireConnected, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldDisconnectedEventToNotOccur()
            {
                Assert.That(this.IsFireDisconnected, Is.EqualTo(false));
            }
        }

        public class WhenConnectSuccessEventHandlerNotExists : TestFixtureBase
        {
            protected override bool HasEventHandler => false;

            [TestCase]
            public void ItShouldConnectedEventToNotOccur()
            {
                Assert.That(this.IsFireConnected, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedEventToNotOccur()
            {
                Assert.That(this.IsFireDisconnected, Is.EqualTo(false));
            }
        }

        public class WhenConnectFailEventHandlerExists : ChannelBaseTest.TestFixtureBase
        {
            protected override bool HasEventHandler => true;

            protected override bool NeedWaitDispose => true;

            protected override bool NeedWaitConnect { get; } = false;

            protected override bool NeedWaitReceiveMessage { get; } = false;

            protected override bool NeedWaitSendMessage { get; } = false;

            protected override WebSocketState WebSocketState => WebSocketState.Aborted;

            [TestCase]
            public void ItShouldConnectedEventToOccur()
            {
                Assert.That(this.IsFireConnected, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedEventToNotOccur()
            {
                Assert.That(this.IsFireDisconnected, Is.EqualTo(true));
            }
        }

        public class WhenConnectFailEventHandlerNotExists : ChannelBaseTest.TestFixtureBase
        {
            protected override bool HasEventHandler => false;

            protected override bool NeedWaitDispose => true;

            protected override bool NeedWaitConnect { get; } = false;

            protected override bool NeedWaitReceiveMessage { get; } = false;

            protected override bool NeedWaitSendMessage { get; } = false;

            protected override WebSocketState WebSocketState => WebSocketState.Aborted;

            [TestCase]
            public void ItShouldConnectedEventToNotOccur()
            {
                Assert.That(this.IsFireConnected, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedEventToNotOccur()
            {
                Assert.That(this.IsFireDisconnected, Is.EqualTo(false));
            }
        }
    }

    namespace OnReceiveMessageTest
    {
        public abstract class TestFixtureBase : ChannelBaseTest.TestFixtureBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
            => new List<ReceiveMessage>()
            {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"message\": { \"content\": \"FooBar\" } }")
            };
        }

        public class WhenConnectSuccessEventHandlerExists : TestFixtureBase
        {
            protected override bool HasEventHandler => true;

            [TestCase]
            public void ItShouldConnectedEventToOccur()
            {
                Assert.That(this.IsFireMessageReceive, Is.EqualTo(true));
            }
        }

        public class WhenConnectSuccessEventHandlerNotExists : TestFixtureBase
        {
            protected override bool HasEventHandler => false;

            [TestCase]
            public void ItShouldConnectedEventToNotOccur()
            {
                Assert.That(this.IsFireMessageReceive, Is.EqualTo(false));
            }
        }
    }
}
