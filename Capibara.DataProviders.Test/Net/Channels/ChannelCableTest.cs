using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using Capibara.Net;
using Capibara.Net.Channels;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Capibara.Test.Net.Channels.ChannelCableTest
{
    public abstract class TestBase : TestFixtureBase
    {
        protected ChannelCable cable;

        protected bool isConnectedEventCalled;

        protected bool isPingReceived;

        protected bool isConfirmSubscriptionReceived;

        protected bool isMessageReceived;

        protected bool isDisconnected;

        protected Task<bool> ConnectTask;

        protected string ReceiveMessage { get; private set; }

        protected virtual Action SendMessage { get; }

        protected virtual Action Connected { get; }

        protected virtual bool NeedEventHandler { get; } = true;

        protected virtual int WebSocketSendBufferSize { get; } = 50;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            using (this.cable = new ChannelCable().BuildUp(this.Container))
            {
                if (this.NeedEventHandler)
                {
                    this.cable.Connected += (sender, e) => this.isConnectedEventCalled = true;
                    this.cable.PingReceived += (sender, e) => this.isPingReceived = true;
                    this.cable.ConfirmSubscriptionReceived += (sender, e) => this.isConfirmSubscriptionReceived = true;
                    this.cable.MessageReceived += (sender, e) =>
                    {
                        this.ReceiveMessage = e;
                        this.isMessageReceived = true;
                    };
                    this.cable.Disconnected += (sender, e) => this.isDisconnected = true;
                }

                // 接続完了を待機
                (this.ConnectTask = cable.Connect()).Wait();

                this.Connected?.Invoke();

                // 受信処理の完了を待機
                Task.WaitAny(Task.Run(() => { while (this.cable.IsOpen) { } }), Task.WhenAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray()));

                // 送信処理を実行する
                this.SendMessage?.Invoke();

                this.cable.Dispose();
            }
        }
    }

    [TestFixture(WebSocketState.Aborted, false)]
    [TestFixture(WebSocketState.Closed, false)]
    [TestFixture(WebSocketState.CloseSent, false)]
    [TestFixture(WebSocketState.Connecting, false)]
    [TestFixture(WebSocketState.CloseReceived, false)]
    [TestFixture(WebSocketState.None, false)]
    [TestFixture(WebSocketState.Open, true)]
    public class ConnectTest : TestBase
    {
        bool result;

        public ConnectTest(WebSocketState state, bool result)
        {
            this.WebSocketState = state;
            this.result = result;
        }

        [TestCase]
        public void ItShouldConnectToExpectedUrl()
        {
            Assert.That(this.WebSocketRequestUrl, Is.EqualTo(this.Environment.WebSocketUrl));
        }

        [TestCase]
        public void ItShouldAuthorizationWithExpected()
        {
            Assert.That(this.WebSocketRequestHeaders.ValueOrDefault("Authorization"), Is.EqualTo($"Token {this.IsolatedStorage.AccessToken}"));
        }

        [TestCase]
        public void ItShouldResultWithExpected()
        {
            Assert.That(this.ConnectTask.Result, Is.EqualTo(this.result));
        }

        [TestCase]
        public void ItShouldDisconnectedToExpected()
        {
            Assert.That(this.isDisconnected, Is.EqualTo(!this.result));
        }
    }

    namespace CloseTest
    {
        public class WhenIsOpen: TestBase
        {
            protected override Action SendMessage
            => () =>
            {
                this.WebSocketState = WebSocketState.Open;
                this.cable.Close().Wait();
            };

            [TestCase]
            public void ItShouldWebSocketCloseCalled()
            {
                Assert.That(this.IsWebSocketCloseCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenIsNotOpen : TestBase
        {
            protected override Action SendMessage
            => () =>
                {
                    this.WebSocketState = WebSocketState.Closed;
                    this.cable.Close().Wait();
                };

            [TestCase]
            public void ItShouldWebSocketCloseNotCalled()
            {
                Assert.That(this.IsWebSocketCloseCalled, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenFail : TestBase
        {
            protected override Action SendMessage
            => () =>
            {
                this.WebSocketState = WebSocketState.Open;
                this.cable.Close().Wait();
            };

            protected override Exception WebSocketCloseException => new Exception();

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Not.Null);
            }
        }
    }

    namespace SendSubscribeTest
    {
        public abstract class SendSubscribeTestBase : TestBase
        {
            bool isMessageSent;

            protected override Action SendMessage
                => () =>
                {
                    this.cable.MessageSent += (sender, e) => this.isMessageSent = true;
                    
                    var channelIdentifier = new Mock<IChannelIdentifier>();
                    channelIdentifier.SetupGet(x => x.Channel).Returns("Channel");

                    cable.SendSubscribe(null);

                    this.SendAsyncSource.Task.Wait();
                };

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldResultWithExpected()
            {
                Assert.That(this.SendAsyncSource.Task.Result.ToSlim(), Is.EqualTo("{\"command\":\"subscribe\",\"identifier\":\"null\"}".ToSlim()));
            }

            [TestCase]
            public void ItShouldMessageSentEventToOccur()
            {
                Assert.That(this.isMessageSent, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSurplus : SendSubscribeTestBase
        {
            protected override int WebSocketSendBufferSize
                => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SubscribeCommand(null))).Length - 1;
        }

        [TestFixture]
        public class WhenJust : SendSubscribeTestBase
        {
            protected override int WebSocketSendBufferSize
                => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new SubscribeCommand(null))).Length;
        }
    }

    namespace ReceiveTest
    {
        public class WhneConnectionClose : TestBase
        {
            protected override bool NeedEventHandler { get; } = false;

            protected override Action Connected =>
                () => this.WebSocketState = WebSocketState.Aborted;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhneReceiveWelcomeWithoutEventHandler : TestBase
        {
            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhneReceivePingWithoutEventHandler : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"type\": \"ping\"}")
                };

            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenReceiveConfirmSubscriptionWithoutEventHandler : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"type\": \"confirm_subscription\"}")
                };

            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenReceiveMessageWithoutEventHandler : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"message\": { \"content\": \"寿限無、寿限無、五劫の擦り切れ、海砂利水魚の、水行末 雲来末 風来末、食う寝る処に住む処、藪ら柑子の藪柑子\" }}")
                };

            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenReceiveMessageWithJapaneseWithoutEventHandler : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"message\": { \"content\": \"FooBar\" } }")
                };

            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenReceiveInvalidMessageWithoutEventHandler : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{}")
                };

            protected override bool NeedEventHandler { get; } = false;

            protected override int WebSocketSendBufferSize { get; } = 2;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenReceiveCloseMessageWithoutEventHandler : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Close, "{\"message\": \"{")
                };

            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhenReceiveNotJsonMessageWithoutEventHandler : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "<html></html>")
                };

            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }
        }

        public class WhneReceiveWelcome : TestBase
        {
            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldConnectedEventToOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToNotOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToNotOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNotOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedToNotOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(false));
            }
        }

        public class WhneReceivePing : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"type\": \"ping\"}")
                };

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldConnectedEventToNotOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToNotOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNotOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedToNotOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(false));
            }
        }

        public class WhenReceiveConfirmSubscription : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"type\": \"confirm_subscription\"}")
                };

            [TestCase]
            public void ItShouldConnectedEventToNotOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToNotOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNotOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedToNotOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(false));
            }
        }

        public class WhenReceiveMessageWithJapanese : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text,  "{\"message\": { \"content\": \"寿限無、寿限無、五劫の擦り切れ、海砂利水魚の、水行末 雲来末 風来末、食う寝る処に住む処、藪ら柑子の藪柑子\" }}")
                };

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldConnectedEventToNotOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToNotOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToNotOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldDisconnectedToNotOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldReceiveMessageWithExpect()
            {
                Assert.That(this.ReceiveMessage.ToSlim(), Is.EqualTo("{ \"content\": \"寿限無、寿限無、五劫の擦り切れ、海砂利水魚の、水行末 雲来末 風来末、食う寝る処に住む処、藪ら柑子の藪柑子\" }".ToSlim()));
            }
        }

        public class WhenReceiveMessage : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text,  "{\"message\": { \"content\": \"FooBar\" } }")
                };

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldConnectedEventToNotOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToNotOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToNotOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldDisconnectedToNotOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldReceiveMessageWithExpect()
            {
                Assert.That(this.ReceiveMessage.ToSlim(), Is.EqualTo("{ \"content\": \"FooBar\" }".ToSlim()));
            }
        }

        public class WhenReceiveInvalidMessage : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{}")
                };

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldConnectedEventToOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToNotOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToNotOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNotOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedToNotOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(false));
            }
        }

        public class WhenReceiveCloseMessage : TestBase
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Close, "{\"message\": \"{")
                };

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldConnectedEventToOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToNotOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToNotOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNotOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedToOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(true));
            }
        }

        public class WhenReceiveNotJsonMessage : TestBase{

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "<html></html>")
                };

            [TestCase]
            public void ItShouldLastExceptionNull()
            {
                Assert.That(this.cable.LastException, Is.Null);
            }

            [TestCase]
            public void ItShouldConnectedEventToOccur()
            {
                Assert.That(this.isConnectedEventCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldPingReceivedEventToNotOccur()
            {
                Assert.That(this.isPingReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldConfirmSubscriptionReceivedEventToNotOccur()
            {
                Assert.That(this.isConfirmSubscriptionReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldMessageReceivedEventToNotOccur()
            {
                Assert.That(this.isMessageReceived, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldDisconnectedToNotOccur()
            {
                Assert.That(this.isDisconnected, Is.EqualTo(false));
            }
        }
    }
}
