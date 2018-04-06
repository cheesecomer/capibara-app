using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.Net;

using Capibara.Test.Net;
using Capibara.Test.Net.Channels.ChannelBaseTest;

using Moq;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Newtonsoft.Json;

namespace Capibara.Test.Models.RoomTest
{
    namespace DeserializeTest
    {
        [TestFixture]
        public class WhenSuccess
        {
            private Room Subject;

            [SetUp]
            public void Setup()
            {
                var json = "{ \"name\": \"AAA\", \"capacity\": 10, \"id\": 99999, \"number_of_participants\": 5 }";
                this.Subject = JsonConvert.DeserializeObject<Room>(json);
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.Subject.Name, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldCapacityWithExpected()
            {
                Assert.That(this.Subject.Capacity, Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(99999));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.Subject.NumberOfParticipants, Is.EqualTo(5));
            }
        }
    }

    namespace RestoreTest
    {
        [TestFixture]
        public class WhenSuccess
        {
            private Room Subject;

            private Room expect;

            [SetUp]
            public void Setup()
            {
                this.expect = new Room { Capacity = 10, Id = 999, Name = "AAA", NumberOfParticipants = 5 };
                this.Subject = new Room();
                this.Subject.Restore(this.expect);
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.Subject.Name, Is.EqualTo(this.expect.Name));
            }

            [TestCase]
            public void ItShouldCapacityWithExpected()
            {
                Assert.That(this.Subject.Capacity, Is.EqualTo(this.expect.Capacity));
            }

            [TestCase]
            public void ItShouldIdWithExpected()
            {
                Assert.That(this.Subject.Id, Is.EqualTo(this.expect.Id));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.Subject.NumberOfParticipants, Is.EqualTo(this.expect.NumberOfParticipants));
            }
        }
    }

    namespace ConnectTest
    {
        [TestFixture]
        public class WhenSuccess : TestFixtureBase
        {
            private Room Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.DoesNotThrowAsync(this.Subject.Connect);
            }
        }

        [TestFixture]
        public class WhenTwiceCall : TestFixtureBase
        {
            private Room Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);
                this.Subject.Connect().Wait();
                this.Subject.BuildUp(this.Container);
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.DoesNotThrowAsync(this.Subject.Connect);
            }
        }
    }

    namespace CloseTest
    {
        [TestFixture]
        public class WhenSuccess : TestFixtureBase
        {
            private Room Subject;

            [SetUp]
            public void Setup()
            {
                this.Subject = new Room().BuildUp(this.Container);
                this.Subject.Connect().Wait();
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.DoesNotThrowAsync(this.Subject.Close);
            }
        }

        [TestFixture]
        public class WhenNotConnected : TestFixtureBase
        {
            private Room Subject;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.DoesNotThrowAsync(this.Subject.Close);
            }
        }
    }

    namespace ReceiveTest
    {
        [TestFixture]
        public class WhenInvalidSystemMessage : TestFixtureBase
        {
            private Room Subject;

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{ \"message\": { \"id\": 0 } }")
                };

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                // 受信完了を待機
                Task.WaitAny(
                    Task.WhenAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray()),
                    Task.Run(() => { while (this.Subject.IsConnected) { } })
                );
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldIsConnected()
            {
                Assert.That(this.Subject.IsConnected, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenUnknownTypeSystemMessage : TestFixtureBase
        {
            private Room Subject;

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{ \"message\": { \"id\": 0}, \"content\": \"{\\\"type\\\":\\\"foo_bar\\\" }\"} }")
                };

            [SetUp]
            public void Setup()
            {
                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                // 受信完了を待機
                Task.WaitAny(
                    Task.WhenAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray()),
                    Task.Run(() => { while (this.Subject.IsConnected) { } })
                );
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldIsConnected()
            {
                Assert.That(this.Subject.IsConnected, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSystemMessageTypeIsEmpty : TestFixtureBase
        {
            private Room Subject;

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{ \"message\": { \"id\": 0}, \"content\": \"{\\\"type\\\":\\\"\\\" }\"} }")
                };

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                // 受信完了を待機
                Task.WaitAny(
                    Task.WhenAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray()),
                    Task.Run(() => { while (this.Subject.IsConnected) { } })
                );
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldIsConnected()
            {
                Assert.That(this.Subject.IsConnected, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccess : TestFixtureBase
        {
            private Room Subject;

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                    new ReceiveMessage(WebSocketMessageType.Text, "{ \"message\": { \"sender\": { \"id\": 10, \"nickname\": \"ABC\" }, \"id\": 99999, \"content\": \"FooBar. Yes!Yes!Yeeeeees!\", \"at\":  \"2017-10-28T20:25:20.000+09:00\" } }")
                };

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                // 受信完了を待機
                Task.WaitAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray());
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldMessagesCountWithExpected()
            {
                Assert.That(this.Subject.Messages.Count, Is.EqualTo(1));
            }
        }

        [TestFixture]
        public class WhenLeaveUser : TestFixtureBase
        {
            private Room Subject;

            protected bool NeedEventHandler { get; set; }

            protected int ExceptParticipantsCount { get; set; } = 0;

            protected bool IsFireJoinUser { get; private set; }

            protected bool IsFireLeaveUser { get; private set; }

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                    new ReceiveMessage(WebSocketMessageType.Text, "{ \"message\": { \"id\": 0,\"content\": \"{\\\"type\\\":\\\"leave_user\\\",\\\"number_of_participants\\\": 1, \\\"user\\\": { \\\"id\\\": 10, \\\"nickname\\\": \\\"ABC\\\" } }\"} }")
                };

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);
                this.Subject.Participants.Add(new User() { Id = 10 });

                if (NeedEventHandler)
                {
                    this.Subject.JoinUser += (sender, e) => this.IsFireJoinUser = true;
                    this.Subject.LeaveUser += (sender, e) => this.IsFireLeaveUser = true;
                }

                this.Subject.Connect().Wait();

                // 受信完了を待機
                Task.WaitAny(
                    Task.WhenAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray()),
                    Task.Run(() => { while (this.Subject.IsConnected) { } })
                );
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldIsConnected()
            {
                Assert.That(this.Subject.IsConnected, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldMessagesCountWithExpected()
            {
                Assert.That(this.Subject.Messages.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.Subject.NumberOfParticipants, Is.EqualTo(1));
            }

            [TestCase]
            public void ItShouldParticipantsCountWithExpected()
            {
                Assert.That(this.Subject.Participants.Count(), Is.EqualTo(this.ExceptParticipantsCount));
            }
        }

        [TestFixture]
        public class WhenLeaveNotExistUser : WhenLeaveUser
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                    new ReceiveMessage(WebSocketMessageType.Text, "{ \"message\": { \"id\": 0,\"content\": \"{\\\"type\\\":\\\"leave_user\\\",\\\"number_of_participants\\\": 1, \\\"user\\\": { \\\"id\\\": 11, \\\"nickname\\\": \\\"ABC\\\" } }\"} }")
            };

            public WhenLeaveNotExistUser()
            {
                this.ExceptParticipantsCount = 1;
            }
        }

        public class WhenLeaveUserWithEventHandler : WhenLeaveUser
        {
            public WhenLeaveUserWithEventHandler()
            {
                this.NeedEventHandler = true;
            }

            [TestCase]
            public void ItShouldJoinUserEventToNotOccur()
            {
                Assert.That(this.IsFireJoinUser, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldLeaveUserEventToOccur()
            {
                Assert.That(this.IsFireLeaveUser, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenJoinUser : TestFixtureBase
        {
            private Room Subject;

            protected virtual bool NeedEventHandler { get; set; }

            protected bool IsFireJoinUser { get; set; }

            protected bool IsFireLeaveUser { get; set; }

            protected int ExceptParticipantsCount { get; set; } = 2;

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"message\": { \"id\": 0, \"content\": \"{\\\"type\\\":\\\"join_user\\\",\\\"number_of_participants\\\": 10, \\\"user\\\": { \\\"id\\\": 10, \\\"nickname\\\": \\\"ABC\\\" } }\"}}")
                };

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room().BuildUp(this.Container);
                this.Subject.Participants.Add(new User() { Id = 11 });

                if (NeedEventHandler)
                {
                    this.Subject.JoinUser += (sender, e) => this.IsFireJoinUser = true;
                    this.Subject.LeaveUser += (sender, e) => this.IsFireLeaveUser = true;
                }

                this.Subject.Connect().Wait();

                // 受信完了を待機
                Task.WaitAny(
                    Task.WhenAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray()),
                    Task.Run(() => { while (this.Subject.IsConnected) { } })
                );
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }

            [TestCase]
            public void ItShouldIsConnected()
            {
                Assert.That(this.Subject.IsConnected, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldMessagesCountWithExpected()
            {
                Assert.That(this.Subject.Messages.Count, Is.EqualTo(0));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.Subject.NumberOfParticipants, Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldParticipantsCountWithExpected()
            {
                Assert.That(this.Subject.Participants.Count(), Is.EqualTo(this.ExceptParticipantsCount));
            }
        }

        [TestFixture]
        public class WhenJoinExitUser : WhenJoinUser
        {
            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                    new ReceiveMessage(WebSocketMessageType.Text, "{ \"message\": { \"id\": 0,\"content\": \"{\\\"type\\\":\\\"join_user\\\",\\\"number_of_participants\\\": 10, \\\"user\\\": { \\\"id\\\": 11, \\\"nickname\\\": \\\"ABC\\\" } }\"} }")
                };

            public WhenJoinExitUser()
            {
                this.ExceptParticipantsCount = 1;
            }
        }

        public class WhenJoinUserWithEventHandler : WhenJoinUser
        {
            public WhenJoinUserWithEventHandler()
            {
                this.NeedEventHandler = true;
            }

            [TestCase]
            public void ItShouldJoinUserEventToOccur()
            {
                Assert.That(this.IsFireJoinUser, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldLeaveUserEventToNotOccur()
            {
                Assert.That(this.IsFireLeaveUser, Is.EqualTo(false));
            }
        }
    }

    namespace SpeakTest
    {
        [TestFixture]
        public abstract class SpeakTestBase : TestFixtureBase
        {
            protected Room Subject;

            protected bool IsSuccess { get; private set; }

            protected bool IsFail { get; private set; }

            protected virtual bool NeedEventHandler { get; } = true;

            protected virtual bool NeedResetSendAsync { get; } = true;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new Room() { Id = 1 }.BuildUp(this.Container);

                if (this.NeedEventHandler)
                {
                    this.Subject.SpeakSuccess += (sender, e) => this.IsSuccess = true;
                    this.Subject.SpeakFail+= (sender, e) => this.IsFail = true;
                }

                // 接続の完了を待機
                this.Subject.Connect().Wait();

                // 接続処理終了を待機
                ConnectTaskSource.Task.Wait();

                // 受信完了を待機
                Task.WaitAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray());

                // 送信完了を待機
                SendAsyncSource.Task.Wait();

                if (this.NeedResetSendAsync)
                    ResetSendAsync();

                this.Subject.Speak("Foo. Bar!").Wait();
            }

            [TearDown]
            public void TearDown()
            {
                this.Subject.Close().Wait();
            }
        }

        [TestFixture]
        public class WhenSuccessWithoutEventHandler : SpeakTestBase
        {
            protected override bool NeedEventHandler { get; } = false;

            [TestCase]
            public void ItShouldRefreshSuccessEventToNotOccur()
            {
                Assert.That(this.IsSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldRefreshFailEventToNotOccur()
            {
                Assert.That(this.IsFail, Is.EqualTo(false));
            }
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

            protected override bool NeedResetSendAsync { get; } = false;

            [TestCase]
            public void ItShouldSuccessEventToNotOccur()
            {
                Assert.That(this.IsSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
            {
                Assert.That(this.IsFail, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailWithEventHandler : SpeakTestBase
        {
            protected override bool NeedResetSendAsync { get; } = false;

            [TestCase]
            public void ItShouldSuccessEventToOccur()
            {
                Assert.That(this.IsSuccess, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldFailEventToNotOccur()
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
            protected Room model;

            protected bool IsRefreshSuccess { get; private set; }

            protected bool IsRefreshFail { get; private set; }

            protected virtual bool NeedEventHandler { get; } = true;

            protected virtual Room Response { get; }

            protected virtual Exception Exception { get; }

            protected bool Result { get; private set; }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.model = new Room { Id = 1 }.BuildUp(this.Container);

                if (this.NeedEventHandler)
                {
                    this.model.RefreshSuccess += (sender, e) => this.IsRefreshSuccess = true;
                    this.model.RefreshFail += (sender, e) => this.IsRefreshFail = true;
                }

                var request = new Mock<RequestBase<Room>>();

                var methodMock = request.Setup(x => x.Execute());

                if (this.Response != null)
                    methodMock.ReturnsAsync(this.Response);
                else if (this.Exception != null)
                    methodMock.ThrowsAsync(this.Exception);
                else
                    throw new ArgumentException();

                this.RequestFactory.Setup(x => x.RoomsShowRequest(It.IsAny<Room>())).Returns(request.Object);

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

            protected override Room Response => new Room();

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
            protected override Room Response => new Room();

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
        public class WhenHasNotMessage : RefreshTestBase
        {
            protected override Room Response => new Room {
                Name = "AAA",
                Capacity = 10,
                NumberOfParticipants = 5
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.model.Name, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldCapacityWithExpected()
            {
                Assert.That(this.model.Capacity, Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.model.NumberOfParticipants, Is.EqualTo(5));
            }

            [TestCase]
            public void ItShouldMessagesCountWithExpected()
            {
                Assert.That(this.model.Messages.Count(), Is.EqualTo(0));
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
            protected override Room Response
            {
                get
                {
                    var result = new Room
                    {
                        Name = "AAA",
                        Capacity = 10,
                        NumberOfParticipants = 5
                    };

                    result.Messages.Add(new Message { Id = 1});
                    result.Messages.Add(new Message { Id = 2});

                    return result;
                }
            }

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.model.Name, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldCapacityWithExpected()
            {
                Assert.That(this.model.Capacity, Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.model.NumberOfParticipants, Is.EqualTo(5));
            }

            [TestCase]
            public void ItShouldMessagesCountWithExpected()
            {
                Assert.That(this.model.Messages.Count(), Is.EqualTo(2));
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
            protected override Room Response
            {
                get
                {
                    var result = new Room
                    {
                        Name = "AAA",
                        Capacity = 10,
                        NumberOfParticipants = 5
                    };

                    result.Messages.Add(new Message { Id = 9999 });
                    result.Messages.Add(new Message { Id = 9999 });

                    return result;
                }
            }


            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.model.Name, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldCapacityWithExpected()
            {
                Assert.That(this.model.Capacity, Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.model.NumberOfParticipants, Is.EqualTo(5));
            }

            [TestCase]
            public void ItShouldMessagesCountWithExpected()
            {
                Assert.That(this.model.Messages.Count(), Is.EqualTo(1));
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
        public class WhenHasNotParticipants : RefreshTestBase
        {
            protected override Room Response => new Room
            {
                Name = "AAA",
                Capacity = 10,
                NumberOfParticipants = 5
            };

            [TestCase]
            public void ItShouldSuccess()
            {
                Assert.That(this.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.model.Name, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldCapacityWithExpected()
            {
                Assert.That(this.model.Capacity, Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.model.NumberOfParticipants, Is.EqualTo(5));
            }

            [TestCase]
            public void ItShouldParticipantsCountWithExpected()
            {
                Assert.That(this.model.Participants.Count, Is.EqualTo(0));
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
        public class WhenHasParticipants : RefreshTestBase
        {
            protected override Room Response
            {
                get
                {
                    var result = new Room
                    {
                        Name = "AAA",
                        Capacity = 10,
                        NumberOfParticipants = 5
                    };

                    result.Participants.Add(new User());

                    return result;
                }
            }

            [TestCase]
            public void ItShouldNameWithExpected()
            {
                Assert.That(this.model.Name, Is.EqualTo("AAA"));
            }

            [TestCase]
            public void ItShouldCapacityWithExpected()
            {
                Assert.That(this.model.Capacity, Is.EqualTo(10));
            }

            [TestCase]
            public void ItShouldNumberOfParticipantsWithExpected()
            {
                Assert.That(this.model.NumberOfParticipants, Is.EqualTo(5));
            }

            [TestCase]
            public void ItShouldParticipantsCountWithExpected()
            {
                Assert.That(this.model.Participants.Count, Is.EqualTo(1));
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

    namespace RejectSubscriptionTest
    {
        [TestFixture]
        public class WhenHasEventHandler : TestFixtureBase
        {
            private Room Subject;

            protected override List<ReceiveMessage> OptionalReceiveMessages
                => new List<ReceiveMessage>()
                {
                new ReceiveMessage(WebSocketMessageType.Text, "{\"type\": \"reject_subscription\"}")
                };

            [TestCase]
            public void ItShouldRejectSubscriptionFire()
            {
                this.Subject = new Room().BuildUp(this.Container);

                bool isRejectSubscription = false;
                this.Subject.RejectSubscription += (s, e) => isRejectSubscription = true;

                this.Subject.Connect().Wait();

                // 受信完了を待機
                Task.WaitAny(
                    Task.WhenAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray()),
                    Task.Run(() => { while (this.Subject.IsConnected) { } })
                );

                Assert.That(isRejectSubscription, Is.EqualTo(true));
            }
        }
    }
}
