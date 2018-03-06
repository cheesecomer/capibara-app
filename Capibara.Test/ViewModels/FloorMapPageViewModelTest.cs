using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.FloorMapPageViewModelTest
{
    [TestFixture]
    public class ItemTappedCommandTest : ViewModelTestBase
    {
        protected string NavigatePageName { get; private set; }

        protected NavigationParameters NavigationParameters { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var navigationService = new Mock<INavigationService>();
            navigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                {
                    this.NavigatePageName = name;
                    this.NavigationParameters = parameters;
                    return Task.Run(() => { });
                });

            var viewModel = new FloorMapPageViewModel(navigationService.Object);

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
            protected FloorMapPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                this.ViewModel = new FloorMapPageViewModel();

                this.ViewModel.BuildUp(container);

                this.ViewModel.RefreshCommand.Execute();

                while(!this.ViewModel.RefreshCommand.CanExecute()) { };
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
            protected override string HttpStabResponse
                => "{\"rooms\": [" +
                    "{ \"id\": 1, \"name\": \"AAA01\", \"capacity\": 11 }," +
                    "{ \"id\": 2, \"name\": \"AAA02\", \"capacity\": 12 }," +
                    "{ \"id\": 3, \"name\": \"AAA03\", \"capacity\": 13 }," +
                    "{ \"id\": 4, \"name\": \"AAA04\", \"capacity\": 14 }," +
                    "{ \"id\": 5, \"name\": \"AAA05\", \"capacity\": 15 }," +
                    "{ \"id\": 6, \"name\": \"AAA06\", \"capacity\": 16 }," +
                    "{ \"id\": 7, \"name\": \"AAA07\", \"capacity\": 17 }," +
                    "{ \"id\": 8, \"name\": \"AAA08\", \"capacity\": 18 }," +
                    "{ \"id\": 9, \"name\": \"AAA09\", \"capacity\": 19 }," +
                    "{ \"id\": 10, \"name\": \"AAA10\", \"capacity\": 0 }," +
                    "{ \"id\": 10, \"name\": \"AAA10\", \"capacity\": 20 }" +
                "] }";

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
                Assert.That(this.ViewModel.Rooms, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenSuccess1 : WhenSuccessBase
        {
            protected override string HttpStabResponse
                => "{\"rooms\": [{ \"name\": \"AAA\", \"capacity\": 10 }] }";

            [TestCase]
            public void ItShouldRoomsWithExpected()
            {
                var comparer = new RoomComparer();
                var expect = new List<Room> { new Room { Name = "AAA", Capacity = 10 } };
                Assert.That(this.ViewModel.Rooms, Is.EqualTo(expect).Using(comparer));
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithService : ViewModelTestBase
        {
            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            protected string NavigatePageName { get; private set; }

            protected bool IsShowDialog { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.NavigatePageName = name;
                        return Task.Run(() => { });
                    });


                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.Run(() => true))
                    .Callback(() => this.IsShowDialog = true);

                var viewModel = new FloorMapPageViewModel(
                    navigationService.Object,
                    pageDialogService.Object);

                viewModel.BuildUp(container);

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
                Assert.That(this.NavigatePageName, Is.EqualTo("/SignInPage"));
            }
        }
    }
}
