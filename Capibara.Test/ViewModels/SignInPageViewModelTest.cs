﻿using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

namespace Capibara.Test.ViewModels.SignInPageViewModelTest
{
    [TestFixture]
    public class SignInCommandCanExecuteTest : TestFixtureBase
    {
        [TestCase("", "", false)]
        [TestCase("", "password", false)]
        [TestCase("", null, false)]
        [TestCase(null, null, false)]
        [TestCase(null, "", false)]
        [TestCase(null, "password", false)]
        [TestCase("email@email.com", "", false)]
        [TestCase("email@email.com", null, false)]
        [TestCase("email@email.com", "password", true)]
        public void ItShouldCanExecuteWithExpected(string email, string password, bool canExecute)
        {
            var viewModel = new SignInPageViewModel().BuildUp(this.GenerateUnityContainer());
            viewModel.Email.Value = email;
            viewModel.Password.Value = password;
            Assert.That(viewModel.SignInCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    namespace SignInCommandExecuteTest
    {
        public abstract class ExecuteTestBase : TestFixtureBase
        {
            protected Task<bool> loginTask;

            protected string navigatePageName;

            protected virtual bool NeedSignInWait { get; } = true;

            protected SignInPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var loginCompletionSource = new TaskCompletionSource<bool>();
                this.loginTask = loginCompletionSource.Task;

                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Callback((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.navigatePageName = name;
                    });

                this.ViewModel = new SignInPageViewModel(navigationService.Object).BuildUp(container);
                this.ViewModel.Email.Value = "user@email.com";
                this.ViewModel.Password.Value = "password";

                this.ViewModel.Model.SignInSuccess += (sender, e) => loginCompletionSource.SetResult(true);
                this.ViewModel.Model.SignInFail += (sender, e) => loginCompletionSource.SetResult(false);

                this.ViewModel.SignInCommand.Execute();

                if (this.NeedSignInWait)
                {
                    this.loginTask.Wait();
                }
            }
        }

        [TestFixture]
        public class WhenWait : ExecuteTestBase
        {
            protected override bool IsInfiniteWait { get; } = true;

            protected override bool NeedSignInWait { get; } = false;

            [TestCase]
            public void ItShouldBusy()
            {
                Assert.That(this.ViewModel.IsBusy.Value, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccess : ExecuteTestBase
        {
            protected override string HttpStabResponse => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\"}";

            [TestCase]
            public void ItShouldSignInSuccess()
            {
                Assert.That(this.loginTask.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNavigateToFloorMap()
            {
                Assert.That(this.navigatePageName, Is.EqualTo("NavigationPage/FloorMapPage"));
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
            public void ItShouldSignInFail()
            {
                Assert.That(this.loginTask.Result, Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldNotNavigate()
            {
                Assert.That(this.navigatePageName, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldNotBusy()
            {
                Assert.That(this.ViewModel.IsBusy.Value, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class SignUpCommandTest : TestFixtureBase
    {
        protected string NavigatePageName { get; private set; }

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
                    navigateTaskSource.SetResult(true);
                });

            var viewModel = new SignInPageViewModel(navigationService.Object);

            viewModel.SignUpCommand.Execute();

            while (!viewModel.SignUpCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToSignUpPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("SignUpPage"));
        }
    }
}
