using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Net.Channels;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Capibara.Test.Net.Channels
{
    namespace ChatChannelIdentifierTest
    {
        public class SerializeTest
        {
            private Dictionary<string, object> actual;

            [SetUp]
            public void Setup()
            {
                var identifier = new ChatChannelIdentifier(10);
                var json = JsonConvert.SerializeObject(identifier);
                this.actual = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }

            [TestCase]
            public void ItShouldChannelWithExpected()
            {
                Assert.That(this.actual.ValueOrDefault("channel"), Is.EqualTo("ChatChannel"));
            }

            [TestCase]
            public void ItShouldRoomIdWithExpected()
            {
                Assert.That(this.actual.ValueOrDefault("room_id"), Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldKeysWithExpected()
            {
                Assert.That(this.actual.Keys.Select(x => x).ToList(), Is.EqualTo(new List<string> { "channel", "room_id" }));
            }
        }
    }

    namespace ChatChannelTest
    {
        public class SpeakTest : TestFixtureBase
        {
            private ChatChannel channel;

            protected Dictionary<string, string> command;

            protected Dictionary<string, object> identifier;

            protected Dictionary<string, object> context;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                (this.channel = new ChatChannel(new Capibara.Models.Room { Id = 1 })).BuildUp(container).Connect();

                // 接続処理終了を待機
                ConnectTaskSource.Task.Wait();

                // 受信完了を待機
                Task.WaitAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray());

                // 送信完了を待機
                SendAsyncSource.Task.Wait();

                ResetSendAsync();

                this.channel.Speak("Foo. Bar!");

                // 送信完了を待機
                SendAsyncSource.Task.Wait();

                var message = SendAsyncSource.Task.Result;
                this.command = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                this.identifier = JsonConvert.DeserializeObject<Dictionary<string, object>>(command.ValueOrDefault("identifier"));
                this.context = JsonConvert.DeserializeObject<Dictionary<string, object>>(command.ValueOrDefault("data"));
            }

            [TestCase]
            public void ItShouldCommandWithExpect()
            {
                Assert.That(this.command.ValueOrDefault("command"), Is.EqualTo("message"));
            }

            [TestCase]
            public void ItShouldIdentifierChannelWithExpected()
            {
                Assert.That(this.identifier.ValueOrDefault("channel"), Is.EqualTo("ChatChannel"));
            }

            [TestCase]
            public void ItShouldIdentifierRoomIdWithExpected()
            {
                Assert.That(this.identifier.ValueOrDefault("room_id"), Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldIdentifierKeysWithExpected()
            {
                Assert.That(this.identifier.Keys.Select(x => x).ToList(), Is.EqualTo(new List<string> { "channel", "room_id" }));
            }

            [TestCase]
            public void ItShouldContextActionWithExpected()
            {
                Assert.That(this.context.ValueOrDefault("action"), Is.EqualTo("speak"));
            }

            [TestCase]
            public void ItShouldContextRoomIdWithExpected()
            {
                Assert.That(this.context.ValueOrDefault("message"), Is.EqualTo("Foo. Bar!"));
            }

            [TestCase]
            public void ItShouldContextKeysWithExpected()
            {
                Assert.That(this.context.Keys.Select(x => x).ToList(), Is.EqualTo(new List<string> { "action", "message" }));
            }
        }

        public class OnConnectedTest : TestFixtureBase
        {
            private ChatChannel channel;

            protected Dictionary<string, string> command;

            protected Dictionary<string, object> identifier;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                (this.channel = new ChatChannel(new Capibara.Models.Room { Id = 1 })).BuildUp(container).Connect();

                // 接続処理終了を待機
                ConnectTaskSource.Task.Wait();

                // 受信完了を待機
                Task.WaitAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray());

                // 送信完了を待機
                SendAsyncSource.Task.Wait();

                var message = SendAsyncSource.Task.Result;
                this.command = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
                this.identifier = JsonConvert.DeserializeObject<Dictionary<string, object>>(command.ValueOrDefault("identifier"));
            }

            [TestCase]
            public void ItShouldCommandWithExpect()
            {
                Assert.That(this.command.ValueOrDefault("command"), Is.EqualTo("subscribe"));
            }

            [TestCase]
            public void ItShouldChannelWithExpected()
            {
                Assert.That(this.identifier.ValueOrDefault("channel"), Is.EqualTo("ChatChannel"));
            }

            [TestCase]
            public void ItShouldRoomIdWithExpected()
            {
                Assert.That(this.identifier.ValueOrDefault("room_id"), Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldKeysWithExpected()
            {
                Assert.That(this.identifier.Keys.Select(x => x).ToList(), Is.EqualTo(new List<string> { "channel", "room_id" }));
            }
        }
    }
}
