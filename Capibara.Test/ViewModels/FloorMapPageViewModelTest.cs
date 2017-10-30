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
    public class ItemTappedCommandTest : TestFixtureBase
    {
        protected string NavigatePageName { get; private set; }

        protected NavigationParameters NavigationParameters { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var navigateTaskSource = new TaskCompletionSource<bool>();
            var navigationService = new Mock<INavigationService>();
            navigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns(navigateTaskSource.Task)
                .Callback((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                {
                    this.NavigatePageName = name;
                    this.NavigationParameters = parameters;
                    navigateTaskSource.SetResult(true);
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
        public class WhenSuccess : TestFixtureBase
        {
            protected Task<bool> refreshTask;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var refreshTaskSource = new TaskCompletionSource<bool>();
                this.refreshTask = refreshTaskSource.Task;

                var viewModel = new FloorMapPageViewModel();

                viewModel.Model.RefreshSuccess += (sender, e) => refreshTaskSource.SetResult(true);
                viewModel.Model.RefreshFail += (sender, e) => refreshTaskSource.SetResult(false);

                viewModel.BuildUp(container);

                viewModel.RefreshCommand.Execute();

                this.refreshTask.Wait();
            }

            [TestCase]
            public void ItShouldRefreshSuccess()
            {
                Assert.That(this.refreshTask.Result, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenUnauthorizedWithService : TestFixtureBase
        {
            protected Task<bool> refreshTask;

            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            protected string NavigatePageName { get; private set; }

            protected bool IsShowDialog { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var refreshTaskSource = new TaskCompletionSource<bool>();
                this.refreshTask = refreshTaskSource.Task;

                var navigateTaskSource = new TaskCompletionSource<bool>();
                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Returns(navigateTaskSource.Task)
                    .Callback((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.NavigatePageName = name;
                        navigateTaskSource.SetResult(true);
                    });


                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.Run(() => true))
                    .Callback(() => this.IsShowDialog = true);

                var viewModel = new FloorMapPageViewModel(
                    navigationService.Object,
                    pageDialogService.Object);

                viewModel.Model.RefreshSuccess += (sender, e) => refreshTaskSource.SetResult(true);
                viewModel.Model.RefreshFail += (sender, e) => refreshTaskSource.SetResult(false);

                viewModel.BuildUp(container);

                viewModel.RefreshCommand.Execute();

                this.refreshTask.Wait();
                navigateTaskSource.Task.Wait();
            }

            [TestCase]
            public void ItShouldRefreshFail()
            {
                Assert.That(this.refreshTask.Result, Is.EqualTo(false));
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
