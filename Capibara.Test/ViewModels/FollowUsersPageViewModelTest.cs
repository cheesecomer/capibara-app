using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Capibara.Models;
using Capibara.Net;
using Capibara.Net.Follows;
using Capibara.ViewModels;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Prism.Navigation;
using SubjectViewModel = Capibara.ViewModels.FollowUsersPageViewModel;

namespace Capibara.Test.ViewModels.FollowUsersPageViewModel
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

                this.RequestFactory.Setup(x => x.FollowsIndexRequest()).Returns(request.Object);

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
        }

        [TestFixture]
        public class WhenSuccess10 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Follows = new List<Follow>
                {
                    new Follow { Id = 1, Target = new User { Id = 11, Nickname = "User001"} },
                    new Follow { Id = 2, Target = new User { Id = 12, Nickname = "User002"} },
                    new Follow { Id = 3, Target = new User { Id = 13, Nickname = "User003"} },
                    new Follow { Id = 4, Target = new User { Id = 14, Nickname = "User004"} },
                    new Follow { Id = 5, Target = new User { Id = 15, Nickname = "User005"} },
                    new Follow { Id = 6, Target = new User { Id = 16, Nickname = "User006"} },
                    new Follow { Id = 7, Target = new User { Id = 17, Nickname = "User007"} },
                    new Follow { Id = 8, Target = new User { Id = 18, Nickname = "User008"} },
                    new Follow { Id = 9, Target = new User { Id = 19, Nickname = "User009"} },
                }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new Models.UserComparer();
                var expect = new List<User>
                {
                    new User { Id = 11, Nickname = "User001"},
                    new User { Id = 12, Nickname = "User002"},
                    new User { Id = 13, Nickname = "User003"},
                    new User { Id = 14, Nickname = "User004"},
                    new User { Id = 15, Nickname = "User005"},
                    new User { Id = 16, Nickname = "User006"},
                    new User { Id = 17, Nickname = "User007"},
                    new User { Id = 18, Nickname = "User008"},
                    new User { Id = 19, Nickname = "User009"},
                };
                Assert.That(this.Subject.FollowUsers.Select(x => x.Model).ToList(), Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Follows = { new Follow { Id = 11, Target = new User { Id = 11, Nickname = "User001" } } }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new Models.UserComparer();
                var expect = new List<User>
                {
                    new User { Id = 11, Nickname = "User001"},
                };
                Assert.That(this.Subject.FollowUsers.Select(x => x.Model).ToList(), Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenExists : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Follows = 
                { 
                    new Follow { Id = 11, Target = new User { Id = 11, Nickname = "User001" } },
                    new Follow { Id = 11, Target = new User { Id = 11, Nickname = "User001" } },
                }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new Models.UserComparer();
                var expect = new List<User>
                {
                    new User { Id = 11, Nickname = "User001"},
                };
                Assert.That(this.Subject.FollowUsers.Select(x => x.Model).ToList(), Is.EqualTo(expect).Using(comparer));
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

                this.RequestFactory.Setup(x => x.FollowsIndexRequest()).Returns(request.Object);

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

                this.RequestFactory.Setup(x => x.FollowsIndexRequest()).Returns(request.Object);

                var subject = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object);

                subject.Object.BuildUp(this.Container);

                subject.Protected().Setup<Task<bool>>("DisplayErrorAlertAsync", exception).ReturnsAsync(true);

                subject.Object.RefreshCommand.Execute();

                while (!subject.Object.RefreshCommand.CanExecute()) { }

                request.Verify(x => x.Execute(), Times.AtLeast(2));
            }
        }
    }
}
