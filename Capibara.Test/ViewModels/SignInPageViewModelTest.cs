using System;
using System.Net;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

namespace Capibara.Test.ViewModels.SignInPageViewModelTest
{
    [TestFixture]
    public class SignInCommandCanExecuteTest : ViewModelTestBase
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

    [TestFixture]
    public class OnSignInSuccessTest : ViewModelTestBase
    {
        protected SignInPageViewModel ViewModel { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();
            var model = new Mock<Session>();
            this.ViewModel = new SignInPageViewModel(this.NavigationService, model: model.Object).BuildUp(container);

            model.Raise(x => x.SignInSuccess += null, EventArgs.Empty);
        }

        [TestCase]
        public void ItShouldNavigateToFloorMap()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("/MainPage/NavigationPage/FloorMapPage"));
        }
    }

    namespace OnSignInFailTest
    {
        public abstract class TestBase : ViewModelTestBase
        {
            protected SignInPageViewModel ViewModel { get; private set; }

            protected abstract Exception Exception { get; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                var model = new Mock<Session>();
                this.ViewModel = new SignInPageViewModel(this.NavigationService, model: model.Object).BuildUp(container);

                model.Raise(x => x.SignInFail += null, new FailEventArgs(this.Exception));
            }
        }

        [TestFixture]
        public class WhenHttpUnauthorizedException : TestBase
        {
            protected override Exception Exception => new HttpUnauthorizedException(HttpStatusCode.Unauthorized, "{ \"message\": \"m9(^Д^)\"}");

            [TestCase]
            public void ItShouldNotNavigate()
            {
                Assert.That(this.NavigatePageName, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldErrorWithExpect()
            {
                Assert.That(this.ViewModel.Error.Value, Is.EqualTo("m9(^Д^)"));
            }
        }

        [TestFixture]
        public class WhenFailWebException : TestBase
        {
            protected override Exception Exception => new WebException();

            [TestCase]
            public void ItShouldNotNavigate()
            {
                Assert.That(this.NavigatePageName, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldErrorIsEmpty()
            {
                Assert.That(this.ViewModel.Error.Value, Is.EqualTo(string.Empty).Or.Null);
            }
        }
    }

    namespace SignInCommandExecuteTest
    {
        public abstract class ExecuteTestBase : ViewModelTestBase
        {
            protected virtual bool NeedSignInWait { get; } = true;

            protected SignInPageViewModel ViewModel { get; private set; }

            protected bool IsSignInCalled;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                var model = new Mock<Session>();
                model.SetupAllProperties();
                model.Setup(x => x.SignIn()).ReturnsAsync(true).Callback(() => this.IsSignInCalled = true);
                this.ViewModel = new SignInPageViewModel(this.NavigationService, model: model.Object).BuildUp(container);
                this.ViewModel.Email.Value = "user@email.com";
                this.ViewModel.Password.Value = "password";

                if (!this.NeedSignInWait)
                {
                    this.ViewModel.SignInCommand.Subscribe(() => new TaskCompletionSource<bool>().Task);
                }

                this.ViewModel.SignInCommand.Execute();
            }

            [TestCase]
            public void ItShouldIsSignInCalled()
            {
                Assert.That(this.IsSignInCalled, Is.EqualTo(true));
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
            [TestCase]
            public void ItShouldNotBusy()
            {
                Assert.That(this.ViewModel.IsBusy.Value, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class SignUpCommandTest : ViewModelTestBase
    {
        [SetUp]
        public void SetUp()
        {
            var viewModel = new SignInPageViewModel(this.NavigationService);

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
