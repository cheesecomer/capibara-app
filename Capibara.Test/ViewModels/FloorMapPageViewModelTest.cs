using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Capibara.Models;
using Capibara.Net;
using Capibara.Net.Rooms;
using Capibara.ViewModels;
using Moq;
using NUnit.Framework;
using Prism.Services;
using SubjectViewModel = Capibara.ViewModels.FloorMapPageViewModel;

namespace Capibara.Test.ViewModels.FloorMapPageViewModel
{
    [TestFixture]
    public class ItemTappedCommandTest : ViewModelTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var viewModel = new SubjectViewModel(this.NavigationService);

            viewModel.ItemTappedCommand.Execute(new Room());

            while (!viewModel.ItemTappedCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("RoomPage"));
        }

        [TestCase]
        public void ItShouldNavigationParametersHsaModel()
        {
            Assert.That(this.NavigationParameters.ContainsKey(ParameterNames.Model), Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldNavigationParameterModelIsRoom()
        {
            Assert.That(this.NavigationParameters[ParameterNames.Model] is Room, Is.EqualTo(true));
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
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
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
            protected override string HttpStabResponse
                => "{\"rooms\": [{ \"name\": \"AAA\", \"capacity\": 10 }] }";

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
        public class WhenUnauthorizedWithService : ViewModelTestBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var request = new Mock<RequestBase<IndexResponse>>();
                request.Setup(x => x.Execute()).ThrowsAsync(new HttpUnauthorizedException(HttpStatusCode.Unauthorized, string.Empty));

                this.RequestFactory.Setup(x => x.RoomsIndexRequest()).Returns(request.Object);

                var viewModel = new SubjectViewModel(this.NavigationService, this.PageDialogService.Object);

                viewModel.BuildUp(this.Container);

                viewModel.RefreshCommand.Execute();

                while(!viewModel.RefreshCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsShowDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNavigateToLogin()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("/SignUpPage"));
            }
        }
    }
}
