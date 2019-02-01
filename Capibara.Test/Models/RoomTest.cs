using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Capibara.Models;
using Capibara.Net;
using Capibara.Net.Channels;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

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

    [TestFixture]
    public class ConnectTest : TestFixtureBase
    {
        private Room Subject;

        private Mock<ChatChannelBase> Channel;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Channel = new Mock<ChatChannelBase>();
            this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

            this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

            this.Subject = new Room().BuildUp(this.Container);

            this.Subject.Connect().Wait();
        }

        [TestCase]
        public void ItShouldCreateChantChannelOnece()
        {
            this.Channel.SetupGet(x => x.IsOpen).Returns(true);
            this.Subject.Connect().Wait();
            this.ChannelFactory.Verify(x => x.CreateChantChannel(It.IsAny<Room>()), Times.Once());
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
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

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
            protected Room Subject;

            protected bool IsSuccess { get; private set; }

            protected bool IsFail { get; private set; }

            protected virtual bool NeedEventHandler { get; } = true;

            protected virtual Exception Exception { get; } = null;

            private Mock<ChatChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);
                var speak = this.Channel.Setup(x => x.Speak(It.IsAny<string>(), It.IsAny<string>()));
                if (this.Exception.IsNull())
                {
                    speak.Returns(Task.CompletedTask);
                }
                else
                {
                    speak.ThrowsAsync(this.Exception);
                }

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                this.Subject = new Room { Id = 1 }.BuildUp(this.Container);

                if (this.NeedEventHandler)
                {
                    this.Subject.SpeakSuccess += (sender, e) => this.IsSuccess = true;
                    this.Subject.SpeakFail += (sender, e) => this.IsFail = true;
                }

                // 接続の完了を待機
                this.Subject.Connect().Wait();

                this.Subject.Speak("Foo. Bar!", string.Empty).Wait();
            }

            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                Assert.DoesNotThrowAsync(() => this.Subject.Speak("Foo. Bar!", string.Empty));
            }

            [TestCase]
            public void ItShouldSpeakCalled()
            {
                this.Channel.Verify(x => x.Speak("Foo. Bar!", string.Empty), Times.Once());
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

                    result.Messages.Add(new Message { Id = 1 });
                    result.Messages.Add(new Message { Id = 2 });

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

    namespace OnMessageReceiveTest
    {
        [TestFixture]
        public class WhenInvalidSystemMessage : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();
            }

            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                Assert.DoesNotThrow(() => this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<Message>(new Message { Id = 0, Content = "" })));
            }
        }

        [TestFixture]
        public class WhenUnknownTypeSystemMessage : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();
            }

            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                Assert.DoesNotThrow(() => this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<Message>(new Message { Id = 0, Content = "{\"type\":\"foo_bar\"}" })));
            }
        }

        [TestFixture]
        public class WhenSystemMessageTypeIsEmpty : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();
            }

            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                Assert.DoesNotThrow(() => this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<Message>(new Message { Id = 0, Content = "{\"type\":\"\"}" })));
            }
        }

        [TestFixture]
        public class WhenSystemMessageTypeIsNull : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();
            }

            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                Assert.DoesNotThrow(() => this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<Message>(new Message { Id = 0, Content = "{\"type\": null}" })));
            }
        }

        [TestFixture]
        public class WhenLeaveUser : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            protected bool NeedEventHandler { get; set; }

            protected int ExceptParticipantsCount { get; set; } = 0;

            protected bool IsFireJoinUser { get; private set; }

            protected bool IsFireLeaveUser { get; private set; }

            protected int UserId { get; set; } = 10;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Participants.Add(new User { Id = this.UserId });

                this.Subject.Connect().Wait();

                if (NeedEventHandler)
                {
                    this.Subject.JoinUser += (sender, e) => this.IsFireJoinUser = true;
                    this.Subject.LeaveUser += (sender, e) => this.IsFireLeaveUser = true;
                }

                var message = new Message { Id = 0, Content = "{\"type\": \"leave_user\", \"number_of_participants\": 1, \"user\": { \"id\": 10, \"nickname\": \"ABC\" } }" };
                
                this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<Message>(message));
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

            public WhenLeaveNotExistUser()
            {
                this.ExceptParticipantsCount = 1;
                this.UserId = 1;
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

            private Mock<ChatChannelBase> Channel;

            protected bool NeedEventHandler { get; set; }

            protected int ExceptParticipantsCount { get; set; } = 2;

            protected bool IsFireJoinUser { get; private set; }

            protected bool IsFireLeaveUser { get; private set; }

            protected int UserId { get; set; } = 11;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Participants.Add(new User { Id = this.UserId });

                this.Subject.Connect().Wait();

                if (NeedEventHandler)
                {
                    this.Subject.JoinUser += (sender, e) => this.IsFireJoinUser = true;
                    this.Subject.LeaveUser += (sender, e) => this.IsFireLeaveUser = true;
                }

                var message = new Message { Id = 0, Content = "{\"type\": \"join_user\", \"number_of_participants\": 10, \"user\": { \"id\": 10, \"nickname\": \"ABC\" } }" };

                this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<Message>(message));
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
            public WhenJoinExitUser()
            {
                this.ExceptParticipantsCount = 1;
                this.UserId = 10;
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

        [TestFixture]
        public class WhenSuccess : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                var message = new Message { Id = 999, Sender = new User(), Content = "FooBar. Yes!Yes!Yeeeeees!", At = DateTimeOffset.Now };

                this.Channel.Raise(x => x.MessageReceive += null, new EventArgs<Message>(message));
            }

            [TestCase]
            public void ItShouldMessagesCountWithExpected()
            {
                Assert.That(this.Subject.Messages.Count, Is.EqualTo(1));
            }
        }
    }

    namespace OnRejectSubscriptionTest
    {
        [TestFixture]
        public class WhenWithEventHandler : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [TestCase]
            public void ItShouldRejectSubscriptionFire()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                bool isRejectSubscription = false;
                this.Subject.RejectSubscription += (s, e) => isRejectSubscription = true;

                this.Subject.Connect().Wait();

                this.Channel.Raise(x => x.RejectSubscription += null, EventArgs.Empty);

                Assert.That(isRejectSubscription, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenWithoutEventHandler : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [TestCase]
            public void ItShouldDoesNotThrow()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                Assert.DoesNotThrow(() => this.Channel.Raise(x => x.RejectSubscription += null, EventArgs.Empty));
            }
        }
    }

    namespace OnConnectedTest
    {
        [TestFixture]
        public class WhenWithEventHandler : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            [TestCase]
            public void ItShouldRejectSubscriptionFire()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                this.Channel.Raise(x => x.Connected += null, EventArgs.Empty);

                Assert.That(this.Subject.IsConnected, Is.EqualTo(true));
            }
        }
    }

    namespace OnDisconnectedTest
    {
        [TestFixture]
        public class WhenWithEventHandler : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            private bool IsDisconnected;

            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Disconnected += (s, e) => this.IsDisconnected = true;

                this.Subject.Connect().Wait();

                this.Channel.Raise(x => x.Disconnected += null, EventArgs.Empty);
            }

            [TestCase]
            public void ItShouldDisconnectedFire()
            {
                Assert.That(this.IsDisconnected, Is.EqualTo(true));
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
        public class WhenWithoutEventHandler : TestFixtureBase
        {
            private Room Subject;

            private Mock<ChatChannelBase> Channel;

            public override void SetUp()
            {
                base.SetUp();

                this.Channel = new Mock<ChatChannelBase>();
                this.Channel.Setup(x => x.Connect()).ReturnsAsync(true);

                this.ChannelFactory.Setup(x => x.CreateChantChannel(It.IsAny<Room>())).Returns(this.Channel.Object);

                this.Subject = new Room().BuildUp(this.Container);

                this.Subject.Connect().Wait();

                this.Channel.Raise(x => x.Disconnected += null, EventArgs.Empty);
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
    }
}
