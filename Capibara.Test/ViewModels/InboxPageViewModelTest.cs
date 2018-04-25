using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Capibara.ViewModels;
using Capibara.Models;
using Capibara.Net;
using Capibara.Net.DirectMessages;

using Moq;
using Moq.Protected;
using NUnit.Framework;

using SubjectViewModel = Capibara.ViewModels.InboxPageViewModel;

namespace Capibara.Test.ViewModels.InboxPageViewModel
{
    namespace RefreshCommandTest
    {
        public abstract class WhenSuccessBase : ViewModelTestBase
        {
            protected SubjectViewModel Subject { get; private set; }

            protected virtual IndexResponse Response { get; } = new IndexResponse();

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var request = new Mock<RequestBase<IndexResponse>>();
                request.Setup(x => x.Execute()).ReturnsAsync(this.Response);

                this.RequestFactory
                    .Setup(x => x.DirectMessagesIndexRequest())
                    .Returns(request.Object);

                this.Subject = new SubjectViewModel();

                this.Subject.BuildUp(this.Container);

                this.Subject.RefreshCommand.Execute();

                while (!this.Subject.RefreshCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
            }

            [TestCase]
            public void ItShouldBlocksIndexExecute()
            {
                this.RequestFactory.Verify(x => x.DirectMessagesIndexRequest(), Times.Once());
            }
        }

        [TestFixture]
        public class WhenSuccess10 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Threads =
                {
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 1 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 2 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 3 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 4 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 5 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 6 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 7 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 8 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                    new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 9 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } },
                }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new DirectMessageThreadComparer();
                var expect = new List<DirectMessageThreadViewModel>
                {
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 1 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 2 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 3 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 4 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 5 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 6 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 7 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 8 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                    new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 9 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }),
                };
                Assert.That(this.Subject.Threads, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Threads = { new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 1 , Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } } }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new DirectMessageThreadComparer();
                var expect = new List<DirectMessageThreadViewModel> { new DirectMessageThreadViewModel(model: new DirectMessageThread { LatestDirectMessage = new DirectMessage { Id = 1, Content = "Message!!!!", At = new DateTimeOffset(2017, 10, 28, 20, 25, 20, TimeSpan.FromHours(9)), Sender = new User { Id = 1, Nickname = "smith" } }, User = new User { Id = 1, Nickname = "smith" } }) };
                Assert.That(this.Subject.Threads, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithService : ViewModelTestBase
        {
            [TestCase]
            public void ItShouldDisplayErrorAlertAsyncCall()
            {
                var exception = new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty);

                var request = new Mock<RequestBase<IndexResponse>>();
                request.Setup(x => x.Execute()).ThrowsAsync(exception);

                this.RequestFactory.Setup(x => x.DirectMessagesIndexRequest()).Returns(request.Object);

                var subject = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object);

                subject.Object.BuildUp(this.Container);

                subject.Object.RefreshCommand.Execute();

                while (!subject.Object.RefreshCommand.CanExecute()) { }

                subject.Protected().Verify<Task<bool>>("DisplayErrorAlertAsync", Times.Once(), exception);
            }
        }

        [TestFixture]
        public class WhenRequestTimeout : ViewModelTestBase
        {
            [TestCase]
            public void ItShouldDisplayErrorAlertAsyncCall()
            {
                var exception = new HttpUnauthorizedException(HttpStatusCode.RequestTimeout, string.Empty);

                var request = new Mock<RequestBase<IndexResponse>>();
                bool isCalled = false;
                request.Setup(x => x.Execute()).Returns(() => {
                    if (isCalled)
                        return Task.FromResult(new IndexResponse());

                    isCalled = true;
                    throw exception;
                });

                this.RequestFactory.Setup(x => x.DirectMessagesIndexRequest()).Returns(request.Object);

                var subject = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object);

                subject.Object.BuildUp(this.Container);

                subject.Protected().Setup<Task<bool>>("DisplayErrorAlertAsync", exception).ReturnsAsync(true);

                subject.Object.RefreshCommand.Execute();

                while (!subject.Object.RefreshCommand.CanExecute()) { }

                request.Verify(x => x.Execute(), Times.AtLeast(2));
            }
        }
    }

    public class DirectMessageThreadComparer : IEqualityComparer<DirectMessageThreadViewModel>
    {
        public bool Equals(DirectMessageThreadViewModel x, DirectMessageThreadViewModel y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return x.At.Value == y.At.Value
                        && x.Content.Value == y.Content.Value
                        && x.Nickname.Value == y.Nickname.Value;
        }

        public int GetHashCode(DirectMessageThreadViewModel room)
        {
            return room.GetHashCode();
        }
    }
}
