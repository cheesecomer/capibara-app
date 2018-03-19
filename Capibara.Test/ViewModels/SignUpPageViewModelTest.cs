using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using OAuthSession = Capibara.Net.OAuth.Session;
using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.SignUpPageViewModelTest
{
    [TestFixture]
    public class SignInCommandCanExecuteTest : ViewModelTestBase
    {
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("a", true)]
        [TestCase(" a ", true)]
        public void ItShouldCanExecuteWithExpected(string nickname, bool canExecute)
        {
            var viewModel = new SignUpPageViewModel().BuildUp(this.GenerateUnityContainer());
            viewModel.Nickname.Value = nickname;
            Assert.That(viewModel.SignUpCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    namespace SignUpCommandExecuteTest
    {
        public abstract class ExecuteTestBase : ViewModelTestBase
        {
            protected virtual bool NeedSignUpWait { get; } = true;

            protected SignUpPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                this.ViewModel = new SignUpPageViewModel(this.NavigationService).BuildUp(container);
                this.ViewModel.Nickname.Value = "Foo.Bar";

                this.ViewModel.SignUpCommand.Execute();

                if (this.NeedSignUpWait)
                {
                    while (!this.ViewModel.SignUpCommand.CanExecute()) { }
                }
            }
        }

        [TestFixture]
        public class WhenWait : ExecuteTestBase
        {
            protected override bool IsInfiniteWait { get; } = true;

            protected override bool NeedSignUpWait { get; } = false;

            [TestCase]
            public void ItShouldBusy()
            {
                Assert.That(this.ViewModel.IsBusy.Value, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccess : ExecuteTestBase
        {
            protected override string HttpStabResponse => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\" }";

            [TestCase]
            public void ItShouldNavigateToFloorMap()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo("/MainPage/NavigationPage/FloorMapPage"));
            }

            [TestCase]
            public void ItShouldNotBusy()
            {
                Assert.That(this.ViewModel.IsBusy.Value, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFail : ExecuteTestBase
        {
            protected override HttpStatusCode HttpStabStatusCode => HttpStatusCode.Unauthorized;

            [TestCase]
            public void ItShouldNotNavigate()
            {
                Assert.That(this.NavigatePageName, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldNotBusy()
            {
                Assert.That(this.ViewModel.IsBusy.Value, Is.EqualTo(false));
            }
        }

        [TestFixture]
        public class WhenFailNetworkError : WhenFail
        {
            protected override Exception RestException => new Exception();
        }
    }

    [TestFixture]
    public class SignInCommandTest : ViewModelTestBase
    {
        [SetUp]
        public void SetUp()
        {
            var viewModel = new SignUpPageViewModel(this.NavigationService);

            viewModel.SignInCommand.Execute();

            while (!viewModel.SignInCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToSignUpPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("SignInPage"));
        }
    }

    namespace OnResume
    {
        [TestFixture]
        public class WhenOAuthCallbackUrlIsPresent : ViewModelTestBase
        {
            private bool IsSignUpWithOAuthCalled;

            [SetUp]
            public void SetUp()
            {
                var model = new Mock<User>();
                model.Setup(
                    x => x.SignUpWithOAuth())
                     .Callback(() => {
                        this.IsSignUpWithOAuthCalled = true;
                     });

                var viewModel = new SignUpPageViewModel(this.NavigationService, model: model.Object);
                viewModel.BuildUp(this.GenerateUnityContainer());

                this.IsolatedStorage.OAuthCallbackUrl = new Uri("foobar://example.com");

                viewModel.OnResume();
            }
            
            [TestCase]
            public void ItShouldNavigateToSignUpPage()
            {
                Assert.That(this.IsSignUpWithOAuthCalled, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenOAuthCallbackUrlIsNotPresent : ViewModelTestBase
        {
            private bool IsSignUpWithOAuthCalled;

            [SetUp]
            public void SetUp()
            {
                var model = new Mock<User>();
                model.Setup(
                    x => x.SignUpWithOAuth())
                     .Callback(() => {
                         this.IsSignUpWithOAuthCalled = true;
                     });

                var viewModel = new SignUpPageViewModel(this.NavigationService);
                viewModel.BuildUp(this.GenerateUnityContainer());

                viewModel.OnResume();
            }

            [TestCase]
            public void ItShouldNavigateToSignUpPage()
            {
                Assert.That(this.IsSignUpWithOAuthCalled, Is.EqualTo(false));
            }
        }
    }

    namespace SignUpWithSnsCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            private ActionSheetButton[] buttons;

            [SetUp]
            public void SetUp()
            {
                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                    .Returns((string name, IActionSheetButton[] buttons) =>
                    {
                        this.buttons = buttons.Select(x => x as ActionSheetButton).ToArray();
                        return Task.Run(() => { });
                    });

                var container = this.GenerateUnityContainer();

                var viewModel = new SignUpPageViewModel(pageDialogService: pageDialogService.Object);

                viewModel.Model.Id = 1;

                viewModel.BuildUp(container);

                viewModel.SignUpWithSnsCommand.Execute();

                while (!viewModel.SignUpWithSnsCommand.CanExecute()) { };
            }

            [TestCase]
            public void ItShouldHasFourButtons()
            {
                Assert.That(this.buttons?.Length, Is.EqualTo(2));
            }

            [TestCase]
            public void ItShouldCancelIsFirstButton()
            {
                Assert.That(this.buttons.ElementAtOrDefault(0).Text, Is.EqualTo("キャンセル"));
            }

            [TestCase]
            public void ItShouldDeleteIsSecondButton()
            {
                Assert.That(this.buttons.ElementAtOrDefault(1).Text, Is.EqualTo("Twitter"));
            }
        }

        [TestFixture]
        public class WhenPressTwitter : ViewModelTestBase
        {
            private ActionSheetButton[] buttons;

            private bool IsOAuthAuthorizeCalled = false;

            [SetUp]
            public void SetUp()
            {
                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                    .Returns((string name, IActionSheetButton[] buttons) =>
                    {
                        this.buttons = buttons.Select(x => x as ActionSheetButton).ToArray();
                        return Task.Run(() => { });
                    });

                var model = new Mock<User>();
                model.Setup(x => x.OAuthAuthorize(OAuthProvider.Twitter)).Callback((OAuthProvider x) => this.IsOAuthAuthorizeCalled = true);

                var container = this.GenerateUnityContainer();

                var viewModel = new SignUpPageViewModel(pageDialogService: pageDialogService.Object, model: model.Object);

                viewModel.BuildUp(container);

                viewModel.SignUpWithSnsCommand.Execute();

                while (!viewModel.SignUpWithSnsCommand.CanExecute()) { };

                this.buttons.ElementAtOrDefault(1)?.Action?.Invoke();
            }

            [TestCase]
            public void ItShouldShowPhotoPicker()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldOAuthAuthorize()
            {
                Assert.That(this.IsOAuthAuthorizeCalled, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenOAuthAuthorizeSuccess : ViewModelTestBase
        {
            private ActionSheetButton[] buttons;

            private bool IsOpenUrlCalled = false;

            [SetUp]
            public void SetUp()
            {
                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                    .Returns((string name, IActionSheetButton[] buttons) =>
                    {
                        this.buttons = buttons.Select(x => x as ActionSheetButton).ToArray();
                        return Task.Run(() => { });
                    });

                var container = this.GenerateUnityContainer();

                this.TwitterOAuthService.Setup(x => x.AuthorizeAsync()).ReturnsAsync(() => new OAuthSession { AuthorizeUri = new Uri("foobar://example.com") });
                this.DeviceService.Setup(x => x.OpenUri(It.Is<Uri>(v => v.ToString() == "foobar://example.com/"))).Callback((Uri x) => this.IsOpenUrlCalled = true);

                var model = new User();
                var eventFired = false;

                model.OAuthAuthorizeFail += (s, e) => eventFired = true;
                model.OAuthAuthorizeSuccess += (s, e) => eventFired = true;

                var viewModel = new SignUpPageViewModel(pageDialogService: pageDialogService.Object, model: model);
                viewModel.BuildUp(container);

                viewModel.SignUpWithSnsCommand.Execute();

                while (!viewModel.SignUpWithSnsCommand.CanExecute()) { };

                this.buttons.ElementAtOrDefault(1)?.Action?.Invoke();

                // イベントが発火するまで待機
                while (!eventFired) { };
            }

            [TestCase]
            public void ItShouldOpenUrl()
            {
                Assert.That(this.IsOpenUrlCalled, Is.EqualTo(true));
            }
        }
    }
}
