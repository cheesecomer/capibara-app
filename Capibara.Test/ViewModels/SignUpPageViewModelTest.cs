using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;

using Moq;
using NUnit.Framework;

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

    [TestFixture]
    public class OnSignUpSuccessTest : ViewModelTestBase
    {
        protected SubjectViewModel Subjet { get; private set; }

        [TestCase(false, "/NavigationPage/AcceptPage")]
        [TestCase(true, "/MainPage/NavigationPage/FloorMapPage")]
        public void ItShouldNavigatePagePathIsExpect(bool isAccepted, string expected)
        {
            var container = this.Container;
            var model = new Mock<User>();
            model.SetupGet(x => x.IsAccepted).Returns(isAccepted);
            this.Subjet = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(container);

            model.Raise(x => x.SignUpSuccess += null, EventArgs.Empty);

            Assert.That(this.NavigatePageName, Is.EqualTo(expected));
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
        public void ItShouldNotNavigate(Exception exception)
        {
            var container = this.Container;
            var model = new Mock<User>();
            var viewModel = new SubjectViewModel(this.NavigationService, this.PageDialogService.Object, model.Object).BuildUp(container);

            model.Raise(x => x.SignUpFail += null, new FailEventArgs(exception));

            Assert.That(this.IsShowDialog, Is.EqualTo(true));
        }
    }

    namespace SignUpCommandExecuteTest
    {
        public abstract class ExecuteTestBase : ViewModelTestBase
        {
            protected virtual bool NeedSignUpWait { get; } = true;

            protected SubjectViewModel Subject { get; private set; }

            protected bool IsSignUpCalled;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<User>();
                model.SetupAllProperties();
                model.Setup(x => x.SignUp()).ReturnsAsync(true).Callback(() => this.IsSignUpCalled = true);

                this.Subject = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.Container);
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
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var viewModel = new SubjectViewModel(this.NavigationService);

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
        [TestFixture(false, "/NavigationPage/AcceptPage")]
        [TestFixture(true, "/MainPage/NavigationPage/FloorMapPage")]
        public class WhenAccessTokenIsPresent : ViewModelTestBase
        {
            private bool IsAccepted;

            private string ExpectedNavigatePageName;

            public WhenAccessTokenIsPresent(bool isAccepted, string expectedNavigatePageName)
            {
                this.IsAccepted = isAccepted;
                this.ExpectedNavigatePageName = expectedNavigatePageName;
            }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var viewModel = new SubjectViewModel(this.NavigationService);
                viewModel.BuildUp(this.Container);

                this.IsolatedStorage.UserId = 1;
                this.IsolatedStorage.AccessToken = Guid.NewGuid().ToString();

                var request = new Mock<RequestBase<User>>();
                request.Setup(x => x.Execute()).ReturnsAsync(new User { IsAccepted = this.IsAccepted });

                this.RequestFactory.Setup(x => x.UsersShowRequest(It.IsAny<User>())).Returns(request.Object);

                viewModel.OnResume();
            }

            [TestCase]
            public void ItShouldNavigatePagePathIsExpect()
            {
                Assert.That(this.NavigatePageName, Is.EqualTo(this.ExpectedNavigatePageName));
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
            }
        }

        [TestFixture]
        public class WhenAccessTokenIsEmpty : ViewModelTestBase
        {
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var viewModel = new SubjectViewModel(this.NavigationService);
                viewModel.BuildUp(this.Container);

                viewModel.OnResume();
            }

            [TestCase]
            public void ItShouldNotNavigate()
            {
                Assert.That(this.NavigatePageName, Is.Null.Or.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldNotShowDialog()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(false));
            }
        }
    }

    [TestFixture]
    public class SignUpWithSnsCommandTest : ViewModelTestBase
    {
        private ActionSheetButton[] buttons;

        private bool IsOpenUrlCalled = false;

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
            Assert.That(this.buttons?.Length, Is.EqualTo(3));
        }

        [TestCase(0, "キャンセル")]
        [TestCase(1, "Twitter")]
        [TestCase(2, "LINE")]
        public void ItShouldButtontTextExpected(int index, string expect)
        {
            Assert.That(this.buttons.ElementAtOrDefault(index).Text, Is.EqualTo(expect));
        }

        [TestCase(1, "http://localhost:9999/api/oauth/twitter")]
        [TestCase(1, "http://localhost:9999/api/oauth/line")]
        public void ItShouldOpenUrl(int index, string url)
        {
            this.DeviceService.Setup(x => x.OpenUri(It.Is<Uri>(v => v.ToString() == url))).Callback((Uri x) => this.IsOpenUrlCalled = true);

            this.buttons.ElementAtOrDefault(index)?.Action?.Invoke();
            Assert.That(this.IsOpenUrlCalled, Is.EqualTo(true));
        }
    }
}
