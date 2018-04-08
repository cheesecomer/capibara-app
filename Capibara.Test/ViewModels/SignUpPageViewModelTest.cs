using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;
using Capibara.Net.Sessions;

using Moq;
using Moq.Protected;
using NUnit.Framework;

using Unity;

using Prism.Navigation;
using Prism.Services;

using SubjectViewModel = Capibara.ViewModels.SignUpPageViewModel;

namespace Capibara.Test.ViewModels.SignUpPageViewModel
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
            var viewModel = new SubjectViewModel().BuildUp(this.Container);
            viewModel.Nickname.Value = nickname;
            Assert.That(viewModel.SignUpCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    namespace OnSignUpSuccessTest
    {
        public abstract class TestBase : ViewModelTestBase
        {
            public virtual bool IsAccepted { get; }

            protected SubjectViewModel Subject { get; private set; }

            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<User>();
                model.SetupGet(x => x.IsAccepted).Returns(IsAccepted);

                this.Subject = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);

                model.Raise(x => x.SignUpSuccess += null, EventArgs.Empty);
            }
        }

        public class WhenAccessTokenIsPresentAndAccepted : TestBase
        {
            public override bool IsAccepted => true;

            [TestCase]
            public void ItShouldNavigateAcceptPage()
            {
                this.NavigationService.Verify(x => x.NavigateAsync("/MainPage/NavigationPage/FloorMapPage", null), Times.Once());
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
                        It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == this.Subject.Model))
                    , Times.Once());
            }
        }
    }

    public class OnSignUpFailTest : ViewModelTestBase
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
        public void ItShouldDisplayErrorAlertAsyncCall(Exception exception)
        {
            var container = this.Container;
            var model = new Mock<User>();
            var viewModel = new Mock<SubjectViewModel>(this.NavigationService.Object, this.PageDialogService.Object, model.Object);
            viewModel.Object.BuildUp(container);

            model.Raise(x => x.SignUpFail += null, new FailEventArgs(exception));

            viewModel.Protected().Verify<Task<bool>>("DisplayErrorAlertAsync", Times.Once(), exception, ItExpr.IsAny<Func<Task>>());
        }
    }

    namespace SignUpCommandExecuteTest
    {
        public abstract class ExecuteTestBase : ViewModelTestBase
        {
            protected virtual bool NeedSignUpWait { get; } = true;

            protected SubjectViewModel Subject { get; private set; }

            protected Mock<User> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Model = new Mock<User>();
                this.Model.SetupAllProperties();
                this.Model.Setup(x => x.SignUp()).ReturnsAsync(true);

                this.Subject = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object).BuildUp(this.Container);
                this.Subject.Nickname.Value = "Foo.Bar";

                if (!this.NeedSignUpWait)
                {
                    this.Subject.SignUpCommand.Subscribe(() => new TaskCompletionSource<bool>().Task);
                }

                this.Subject.SignUpCommand.Execute();
            }

            [TestCase]
            public void ItShouldIsSignUpCalled()
            {
                this.Model.Verify(x => x.SignUp(), Times.Once());
            }
        }

        [TestFixture]
        public class WhenWait : ExecuteTestBase
        {
            protected override bool NeedSignUpWait { get; } = false;

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
    public class SignInCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldNavigateToSignUpPage()
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.SignInCommand.Execute();

            while (!viewModel.SignInCommand.CanExecute()) { }

            this.NavigationService.Verify(x => x.NavigateAsync("SignInPage", null, null, false), Times.Once());
        }
    }

    namespace OnResume
    {
        public abstract class WhenAccessTokenIsPresent : ViewModelTestBase
        {
            public virtual bool IsAccepted { get; }

            public SubjectViewModel Subject { get; private set; }

            public CreateResponse Response;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Subject = new SubjectViewModel(this.NavigationService.Object);
                this.Subject.BuildUp(this.Container);

                this.IsolatedStorage.UserId = 1;
                this.IsolatedStorage.AccessToken = Guid.NewGuid().ToString();

                var request = new Mock<RequestBase<CreateResponse>>();
                request.Setup(x => x.Execute()).ReturnsAsync(this.Response = new CreateResponse { IsAccepted = this.IsAccepted });

                this.RequestFactory.Setup(x => x.SessionsRefreshRequest()).Returns(request.Object);

                this.Subject.OnResume();
            }
        }

        public class WhenAccessTokenIsPresentAndAccepted : WhenAccessTokenIsPresent
        {
            public override bool IsAccepted => true;

            [TestCase]
            public void ItShouldNavigateAcceptPage()
            {
                this.NavigationService.Verify(x => x.NavigateAsync("/MainPage/NavigationPage/FloorMapPage", null), Times.Once());
            }
        }
        public class WhenAccessTokenIsPresentAndNotAccepted : WhenAccessTokenIsPresent
        {
            public override bool IsAccepted => false;

            [TestCase]
            public void ItShouldNavigateAcceptPage()
            {
                this.NavigationService.Verify(
                    x => x.NavigateAsync(
                        "/NavigationPage/AcceptPage",
                        It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == this.Response))
                    , Times.Once());
            }
        }

        [TestFixture]
        public class WhenAccessTokenIsEmpty : ViewModelTestBase
        {
            [TestCase]
            public void ItShouldSessionsRefreshNotCall()
            {
                var viewModel = new SubjectViewModel(this.NavigationService.Object);
                viewModel.BuildUp(this.Container);

                viewModel.OnResume();

                this.RequestFactory.Verify(x => x.SessionsRefreshRequest(), Times.Never());
            }
        }
    }

    [TestFixture]
    public class SignUpWithSnsCommandTest : ViewModelTestBase
    {
        private ActionSheetButton[] buttons;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.PageDialogService
                .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                .Returns((string name, IActionSheetButton[] buttons) =>
                {
                    this.buttons = buttons.Select(x => x as ActionSheetButton).ToArray();
                    return Task.Run(() => { });
                });

            var viewModel = new SubjectViewModel(pageDialogService: this.PageDialogService.Object);

            viewModel.Model.Id = 1;

            viewModel.BuildUp(this.Container);

            viewModel.SignUpWithSnsCommand.Execute();

            while (!viewModel.SignUpWithSnsCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldHasFourButtons()
        {
            Assert.That(this.buttons?.Length, Is.EqualTo(4));
        }

        [TestCase(0, "キャンセル")]
        [TestCase(1, "Google")]
        [TestCase(2, "Twitter")]
        [TestCase(3, "LINE")]
        public void ItShouldButtontTextExpected(int index, string expect)
        {
            Assert.That(this.buttons.ElementAtOrDefault(index).Text, Is.EqualTo(expect));
        }

        [TestCase(1, "http://localhost:9999/api/oauth/google")]
        [TestCase(2, "http://localhost:9999/api/oauth/twitter")]
        [TestCase(3, "http://localhost:9999/api/oauth/line")]
        public void ItShouldOpenUrl(int index, string url)
        {
            this.buttons.ElementAtOrDefault(index)?.Action?.Invoke();
            this.SnsLoginService.Verify(x => x.Open(new Uri(url).AbsoluteUri), Times.Once());
        }
    }
}
