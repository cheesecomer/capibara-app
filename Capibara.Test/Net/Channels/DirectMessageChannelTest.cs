﻿using System;
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
using NUnit.Framework;
using Newtonsoft.Json;

using SpeakActionContext = Capibara.Net.Channels.DirectMessageChannel.SpeakActionContext;

namespace Capibara.Test.Net.Channels
{
    namespace DirectMessageChannelIdentifierTest
    {
        public class SerializeTest
        {
            private Dictionary<string, object> Subject;

            [SetUp]
            public void Setup()
            {
                var identifier = new DirectMessageChannelIdentifier(10);
                var json = JsonConvert.SerializeObject(identifier);
                this.Subject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }

            [TestCase]
            public void ItShouldChannelWithExpected()
            {
                Assert.That(this.Subject.ValueOrDefault("channel"), Is.EqualTo("DirectMessageChannel"));
            }

            [TestCase]
            public void ItShouldRoomIdWithExpected()
            {
                Assert.That(this.Subject.ValueOrDefault("addressee_id"), Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldKeysWithExpected()
            {
                Assert.That(this.Subject.Keys.Select(x => x).ToList(), Is.EqualTo(new List<string> { "channel", "addressee_id" }));
            }
        }
    }

    namespace DirectMessageChannelTest
    {
        namespace SpeakActionContextTest
        {
            public class SerializeTest
            {
                private Dictionary<string, object> Subject;

                [SetUp]
                public void Setup()
                {
                    var context = new SpeakActionContext { Message = "Hello!" };
                    var json = JsonConvert.SerializeObject(context);
                    this.Subject = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                }

                [TestCase]
                public void ItShouldActionWithExpected()
                {
                    Assert.That(this.Subject.ValueOrDefault("action"), Is.EqualTo("speak"));
                }

                [TestCase]
                public void ItShouldMessageWithExpected()
                {
                    Assert.That(this.Subject.ValueOrDefault("message"), Is.EqualTo("Hello!"));
                }
            }
        }

        public class ChannelIdentifierPropertyTest
        {
            private DirectMessageChannel Subject;

            [SetUp]
            public void SetUp()
            {
                this.Subject = new DirectMessageChannel(new Capibara.Models.User { Id = 1 });
            }

            [TestCase]
            public void ItShouldDirectMessageChannelIdentifier()
            {
                Assert.That(this.Subject.ChannelIdentifier, Is.TypeOf<DirectMessageChannelIdentifier>());
            }

            [TestCase]
            public void ItShouldRoomIdExpect()
            {
                Assert.That((this.Subject.ChannelIdentifier as DirectMessageChannelIdentifier)?.AddresseeId, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldCreateOnce()
            {
                var origin = this.Subject.ChannelIdentifier;
                Assert.That(this.Subject.ChannelIdentifier, Is.EqualTo(origin));
            }
        }

        public class SpeakTest : TestFixtureBase
        {
            private DirectMessageChannel Subject;

            [TestCase]
            public void ItShouldSendCommandCalled()
            {
                this.Subject = new DirectMessageChannel(new Capibara.Models.User { Id = 1 }).BuildUp(this.Container);

                var cable = new Mock<ChannelCableBase>();
                cable
                    .Setup(x => x.SendCommand(It.IsAny<MessageCommand<SpeakActionContext>>()))
                    .Returns(Task.CompletedTask);

                this.ChannelCableFactory.Setup(x => x.Create()).Returns(cable.Object);

                this.Subject.Connect();
                this.Subject.Speak("Hello!");

                cable.Verify(
                    x => x.SendCommand(It.Is<MessageCommand<SpeakActionContext>>(
                        v => v.Identifier == this.Subject.ChannelIdentifier && v.Context.Message == "Hello!")),
                    Times.Once());
            }
        }
    }
}
