using System;
using System.Net;
using System.Threading.Tasks;

using Capibara.Net;
using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using Moq.Protected;
using NUnit.Framework;

using Unity;
using Unity.Extension;

using Prism.Navigation;

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

    namespace OnSignInSuccessTest
    {
        public abstract class TestBase : ViewModelTestBase
        {
            public virtual bool IsAccepted { get; }

            protected SubjectViewModel Subject { get; private set; }

            protected User User;

            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<Session>();
                model.SetupGet(x => x.IsAccepted).Returns(IsAccepted);

                this.Subject = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);

                this.Container.RegisterInstance(typeof(User), UnityInstanceNames.CurrentUser, this.User = new User());

                model.Raise(x => x.SignInSuccess += null, EventArgs.Empty);
            }
        }

        public class WhenAccessTokenIsPresentAndAccepted : TestBase
        {
            public override bool IsAccepted => true;

            [TestCase]
            public void ItShouldNavigateAcceptPage()
            {
                this.NavigationService.Verify(x => x.NavigateAsync("/NavigationPage/MainPage", null), Times.Once());
            }
        }

        public class WhenAccessTokenIsPresentAndNotAccepted : TestBase
        {
            public override bool IsAccepted => false;

            [TestCase]
            public void ItShouldNavigateAcceptPage()
            {
                this.NavigationService.Verify(
                    x => x.NavigateAsync(
                        "/NavigationPage/AcceptPage",
                        It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == this.User))
                    , Times.Once());
            }
        }
    }

    namespace OnSignInFailTest
    {
        public abstract class TestBase : ViewModelTestBase
        {
            protected Mock<SubjectViewModel> Subject { get; private set; }

            protected abstract Exception Exception { get; }

            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<Session>();
                this.Subject = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object, model.Object);
                this.Subject.Object.BuildUp(this.Container);

                model.Raise(x => x.SignInFail += null, new FailEventArgs(this.Exception));
            }
        }

        [TestFixture]
        public class WhenHttpUnauthorizedException : TestBase
        {
            protected override Exception Exception { get; } = new HttpUnauthorizedException(HttpStatusCode.Unauthorized, "{ \"message\": \"m9(^Д^)\"}");

            [TestCase]
            public void ItShouldErrorMessageExpect()
            {
                Assert.That(this.Subject.Object.Error.Value, Is.EqualTo("m9(^Д^)"));
            }
        }

        [TestFixture]
        public class WhenFailWebException : TestBase
        {
            protected override Exception Exception { get; } = new WebException();

            [TestCase]
            public void ItShouldDisplayErrorAlertAsyncCall()
            {
                this.Subject.Protected().Verify<Task<bool>>("DisplayErrorAlertAsync", Times.Once(), this.Exception);
            }
        }
    }

    namespace SignInCommandExecuteTest
    {
        public abstract class ExecuteTestBase : ViewModelTestBase
        {
            protected virtual bool NeedSignInWait { get; } = true;

            protected SubjectViewModel Subject { get; private set; }

            protected Mock<Session> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Model = new Mock<Session>();
                this.Model.SetupAllProperties();
                this.Model.Setup(x => x.SignIn()).ReturnsAsync(true);

                this.Subject = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, this.Model.Object).BuildUp(this.Container);
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
                this.Model.Verify(x => x.SignIn(), Times.Once());
            }
        }

        [TestFixture]
        public class WhenWait : ExecuteTestBase
        {
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
        [TestCase]
        public void ItShouldNavigateToSignUpPage()
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object);

            viewModel.SignUpCommand.Execute();

            while (!viewModel.SignUpCommand.CanExecute()) { }

            this.NavigationService.Verify(x => x.NavigateAsync("SignUpPage", null, null, false), Times.Once());
        }
    }
}
