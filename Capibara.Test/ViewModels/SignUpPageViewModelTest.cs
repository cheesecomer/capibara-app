﻿using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using OAuthSession = Capibara.Net.OAuth.Session;
using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.SignUpPageViewModelTest
{
    [TestFixture]
    public class SignUpCommandCanExecuteTest : ViewModelTestBase
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

    [TestFixture]
    public class OnSignInSuccessTest : ViewModelTestBase
    {
        protected SignUpPageViewModel ViewModel { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();
            var model = new Mock<User>();
            this.ViewModel = new SignUpPageViewModel(this.NavigationService, model: model.Object).BuildUp(container);

            model.Raise(x => x.SignUpSuccess += null, EventArgs.Empty);
        }

        [TestCase]
        public void ItShouldNavigateToFloorMap()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("/MainPage/NavigationPage/FloorMapPage"));
        }
    }

    namespace OnSignInFailTest
    {
        public class OnSignInFailTest : ViewModelTestBase
        {
            public static object[][] TestCaseSource()
            {
                return new object[][]
                {
                    new [] { new HttpUnauthorizedException(HttpStatusCode.Unauthorized, "{ \"message\": \"m9(^Д^)\"}") },
                    new [] { new WebException() }
                };
            }

            [TestCaseSource("TestCaseSource")]
            public void ItShouldNotNavigate(Exception exception)
            {
                var container = this.GenerateUnityContainer();
                var model = new Mock<User>();
                var viewModel = new SignUpPageViewModel(this.NavigationService, model: model.Object).BuildUp(container);

                model.Raise(x => x.SignUpFail += null, new FailEventArgs(exception));

                Assert.That(this.NavigatePageName, Is.Null.Or.EqualTo(string.Empty));
            }
        }
    }

    namespace SignUpCommandExecuteTest
    {
        public abstract class ExecuteTestBase : ViewModelTestBase
        {
            protected virtual bool NeedSignUpWait { get; } = true;

            protected SignUpPageViewModel ViewModel { get; private set; }

            protected bool IsSignUpCalled;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                var model = new Mock<User>();
                model.SetupAllProperties();
                model.Setup(x => x.SignUp()).ReturnsAsync(true).Callback(() => this.IsSignUpCalled = true);

                this.ViewModel = new SignUpPageViewModel(this.NavigationService, model: model.Object).BuildUp(container);
                this.ViewModel.Nickname.Value = "Foo.Bar";

                if (!this.NeedSignUpWait)
                {
                    this.ViewModel.SignUpCommand.Subscribe(() => new TaskCompletionSource<bool>().Task);
                }

                this.ViewModel.SignUpCommand.Execute();
            }

            [TestCase]
            public void ItShouldIsSignUpCalled()
            {
                Assert.That(this.IsSignUpCalled, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenWait : ExecuteTestBase
        {
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
            [TestCase]
            public void ItShouldNotBusy()
            {
                Assert.That(this.ViewModel.IsBusy.Value, Is.EqualTo(false));
            }
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
                model.Setup(x => x.SignUpWithOAuth()).Callback(() => this.IsSignUpWithOAuthCalled = true);

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
                model.Setup(x => x.SignUpWithOAuth()).Callback(() => this.IsSignUpWithOAuthCalled = true);

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
            public void ItShouldTwitterIsSecondButton()
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
    }

    public class OAuthAuthorizeSuccessTest : ViewModelTestBase
    {
        private bool IsOpenUrlCalled = false;

        [TestCase]
        public void ItShouldOpenUrl()
        {
            var container = this.GenerateUnityContainer();

            this.DeviceService.Setup(x => x.OpenUri(It.Is<Uri>(v => v.ToString() == "foobar://example.com/"))).Callback((Uri x) => this.IsOpenUrlCalled = true);

            var model = new Mock<User>();
            var viewModel = new SignUpPageViewModel(model: model.Object).BuildUp(container);

            model.Raise(x => x.OAuthAuthorizeSuccess += null, new EventArgs<Uri>(new Uri("foobar://example.com/")));

            Assert.That(this.IsOpenUrlCalled, Is.EqualTo(true));
        }
    }
}
