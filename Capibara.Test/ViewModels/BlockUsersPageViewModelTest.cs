using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;
using Capibara.Net.Blocks;

using Moq;
using Moq.Protected;
using NUnit.Framework;

using Prism.Services;

using SubjectViewModel = Capibara.ViewModels.BlockUsersPageViewModel;

namespace Capibara.Test.ViewModels.BlockUsersPageViewModel
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
                    .Setup(x => x.BlocksIndexRequest())
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
                this.RequestFactory.Verify(x => x.BlocksIndexRequest(), Times.Once());
            }
        }

        [TestFixture]
        public class WhenSuccess10 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Blocks =
                {
                    new Block { Id = 1, Target = new User { Id = 11, Nickname = "ABC1"} },
                    new Block { Id = 2, Target = new User { Id = 12, Nickname = "ABC2"} },
                    new Block { Id = 3, Target = new User { Id = 13, Nickname = "ABC3"} },
                    new Block { Id = 4, Target = new User { Id = 14, Nickname = "ABC4"} },
                    new Block { Id = 5, Target = new User { Id = 15, Nickname = "ABC5"} },
                    new Block { Id = 6, Target = new User { Id = 16, Nickname = "ABC6"} },
                    new Block { Id = 7, Target = new User { Id = 17, Nickname = "ABC7"} },
                    new Block { Id = 8, Target = new User { Id = 18, Nickname = "ABC8"} },
                    new Block { Id = 9, Target = new User { Id = 19, Nickname = "ABC9"} },
                    new Block { Id = 10, Target = new User { Id = 20, Nickname = "ABC0"} }
                }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new BlockComparer();
                var expect = new List<Block>
                {
                    new Block { Id = 1, Target = new User { Id = 11, Nickname = "ABC1"} },
                    new Block { Id = 2, Target = new User { Id = 12, Nickname = "ABC2"} },
                    new Block { Id = 3, Target = new User { Id = 13, Nickname = "ABC3"} },
                    new Block { Id = 4, Target = new User { Id = 14, Nickname = "ABC4"} },
                    new Block { Id = 5, Target = new User { Id = 15, Nickname = "ABC5"} },
                    new Block { Id = 6, Target = new User { Id = 16, Nickname = "ABC6"} },
                    new Block { Id = 7, Target = new User { Id = 17, Nickname = "ABC7"} },
                    new Block { Id = 8, Target = new User { Id = 18, Nickname = "ABC8"} },
                    new Block { Id = 9, Target = new User { Id = 19, Nickname = "ABC9"} },
                    new Block { Id = 10, Target = new User { Id = 20, Nickname = "ABC0"} },
                };
                Assert.That(this.Subject.Blocks, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Blocks = { new Block { Id = 10, Target = new User { Id = 10, Nickname = "ABC"} } }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new BlockComparer();
                var expect = new List<Block> { new Block { Id = 10, Target = new User { Id = 10, Nickname = "ABC"} } };
                Assert.That(this.Subject.Blocks, Is.EqualTo(expect).Using(comparer));
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

                this.RequestFactory.Setup(x => x.BlocksIndexRequest()).Returns(request.Object);

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

                this.RequestFactory.Setup(x => x.BlocksIndexRequest()).Returns(request.Object);

                var subject = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object);

                subject.Object.BuildUp(this.Container);

                subject.Protected().Setup<Task<bool>>("DisplayErrorAlertAsync", exception).ReturnsAsync(true);

                subject.Object.RefreshCommand.Execute();

                while (!subject.Object.RefreshCommand.CanExecute()) { }

                request.Verify(x => x.Execute(), Times.AtLeast(2));
            }
        }
    }

    public class BlockComparer: IEqualityComparer<Block>
    {
        public bool Equals(Block x, Block y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null | y == null)
                return false;
            else
                return
                    x.Id == y.Id
                 && x.Target?.Id == y.Target?.Id
                 && x.Target?.Nickname == y.Target?.Nickname;
        }

        public int GetHashCode(Block room)
        {
            return room.GetHashCode();
        }
    }
}
