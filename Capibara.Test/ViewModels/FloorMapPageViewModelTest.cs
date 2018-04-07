using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Capibara.Models;
using Capibara.Net;
using Capibara.Net.Rooms;
using Capibara.ViewModels;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Prism.Navigation;
using SubjectViewModel = Capibara.ViewModels.FloorMapPageViewModel;

namespace Capibara.Test.ViewModels.FloorMapPageViewModel
{
    [TestFixture]
    public class ItemTappedCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            var room = new Room();
            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.ItemTappedCommand.Execute(room);

            while (!viewModel.ItemTappedCommand.CanExecute()) { }
            this.NavigationService.Verify(
                x => x.NavigateAsync(
                    "RoomPage",
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == room))
                , Times.Once());
        }
    }

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

                this.RequestFactory.Setup(x => x.RoomsIndexRequest()).Returns(request.Object);

                this.Subject = new SubjectViewModel();

                this.Subject.BuildUp(this.Container);

                this.Subject.RefreshCommand.Execute();

                while(!this.Subject.RefreshCommand.CanExecute()) { }
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
                Rooms = new List<Room>
                {
                    new Room { Id = 1, Name ="AAA01", Capacity = 11 },
                    new Room { Id = 2, Name ="AAA02", Capacity = 12 },
                    new Room { Id = 3, Name ="AAA03", Capacity = 13 },
                    new Room { Id = 4, Name ="AAA04", Capacity = 14 },
                    new Room { Id = 5, Name ="AAA05", Capacity = 15 },
                    new Room { Id = 6, Name ="AAA06", Capacity = 16 },
                    new Room { Id = 7, Name ="AAA07", Capacity = 17 },
                    new Room { Id = 8, Name ="AAA08", Capacity = 18 },
                    new Room { Id = 9, Name ="AAA09", Capacity = 19 },
                    new Room { Id = 10, Name ="AAA10", Capacity = 20 },
                }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new RoomComparer();
                var expect = new List<Room>
                {
                    new Room { Id = 1, Name ="AAA01", Capacity = 11 },
                    new Room { Id = 2, Name ="AAA02", Capacity = 12 },
                    new Room { Id = 3, Name ="AAA03", Capacity = 13 },
                    new Room { Id = 4, Name ="AAA04", Capacity = 14 },
                    new Room { Id = 5, Name ="AAA05", Capacity = 15 },
                    new Room { Id = 6, Name ="AAA06", Capacity = 16 },
                    new Room { Id = 7, Name ="AAA07", Capacity = 17 },
                    new Room { Id = 8, Name ="AAA08", Capacity = 18 },
                    new Room { Id = 9, Name ="AAA09", Capacity = 19 },
                    new Room { Id = 10, Name ="AAA10", Capacity = 20 },
                };
                Assert.That(this.Subject.Rooms, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Rooms = { new Room { Name = "AAA", Capacity = 10 } }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new RoomComparer();
                var expect = new List<Room> { new Room { Name = "AAA", Capacity = 10 } };
                Assert.That(this.Subject.Rooms, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenExists : WhenSuccessBase
        {
            protected override IndexResponse Response => new IndexResponse
            {
                Rooms =
                {
                    new Room { Id = 1, Name = "AAA", Capacity = 10 },
                    new Room { Id = 1, Name = "AAA", Capacity = 10 }
                }
            };

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new RoomComparer();
                var expect = new List<Room> { new Room { Id = 1, Name = "AAA", Capacity = 10 } };
                Assert.That(this.Subject.Rooms, Is.EqualTo(expect).Using(comparer));
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

                this.RequestFactory.Setup(x => x.RoomsIndexRequest()).Returns(request.Object);

                var subject = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object);

                subject.Object.BuildUp(this.Container);

                subject.Object.RefreshCommand.Execute();

                while (!subject.Object.RefreshCommand.CanExecute()) { }

                subject.Protected().Verify<Task<bool>>("DisplayErrorAlertAsync", Times.Once(), exception, ItExpr.IsAny<Func<Task>>());
            }
        }
    }
}
