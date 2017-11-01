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
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var viewModel = new FloorMapPageViewModel();

                viewModel.BuildUp(container);

                viewModel.RefreshCommand.Execute();

                while(!viewModel.RefreshCommand.CanExecute()) { };
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
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
