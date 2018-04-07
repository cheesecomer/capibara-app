using System;
using System.Net;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Models;

using Moq;
using NUnit.Framework;

using SubjectViewModel = Capibara.ViewModels.SignInPageViewModel;

namespace Capibara.Test.ViewModels.SignInPageViewModel
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
            var viewModel = new SubjectViewModel().BuildUp(this.Container);
            viewModel.Email.Value = email;
            viewModel.Password.Value = password;
            Assert.That(viewModel.SignInCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    public class OnSignInSuccessTest : ViewModelTestBase
    {
        protected SubjectViewModel Subjet { get; private set; }

        [TestCase(false, "/NavigationPage/AcceptPage")]
        [TestCase(true, "/MainPage/NavigationPage/FloorMapPage")]
        public void ItShouldNavigatePagePathIsExpect(bool isAccepted, string expected)
        {
            var container = this.Container;
            var model = new Mock<Session>();
            model.SetupGet(x => x.IsAccepted).Returns(isAccepted);
            this.Subjet = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(container);

            model.Raise(x => x.SignInSuccess += null, EventArgs.Empty);

            Assert.That(this.NavigatePageName, Is.EqualTo(expected));
        }
    }

    namespace OnSignInFailTest
    {
        public abstract class TestBase : ViewModelTestBase
        {
            protected SubjectViewModel Subject { get; private set; }

            protected abstract Exception Exception { get; }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<Session>();
                this.Subject = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, model.Object).BuildUp(this.Container);

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
                Assert.That(this.Subject.Error.Value, Is.EqualTo("m9(^Д^)"));
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
                Assert.That(this.Subject.Error.Value, Is.EqualTo(string.Empty).Or.Null);
            }
        }
    }

    namespace SignInCommandExecuteTest
    {
        public abstract class ExecuteTestBase : ViewModelTestBase
        {
            protected virtual bool NeedSignInWait { get; } = true;

            protected SubjectViewModel Subject { get; private set; }

            protected bool IsSignInCalled;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<Session>();
                model.SetupAllProperties();
                model.Setup(x => x.SignIn()).ReturnsAsync(true).Callback(() => this.IsSignInCalled = true);
                this.Subject = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, model.Object).BuildUp(this.Container);
                this.Subject.Email.Value = "user@email.com";
                this.Subject.Password.Value = "password";

                if (!this.NeedSignInWait)
                {
                    this.Subject.SignInCommand.Subscribe(() => new TaskCompletionSource<bool>().Task);
                }

                this.Subject.SignInCommand.Execute();
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
                Assert.That(this.Subject.IsBusy.Value, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenSuccess : ExecuteTestBase
        {
            [TestCase]
            public void ItShouldNotBusy()
            {
                Assert.That(this.Subject.IsBusy.Value, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class SignUpCommandTest : ViewModelTestBase
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object);

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
