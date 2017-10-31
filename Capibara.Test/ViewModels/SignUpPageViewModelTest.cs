using System.Net;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

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
            protected Task<bool> signUpTask;

            protected string navigatePageName;

            protected virtual bool NeedSignUpWait { get; } = true;

            protected SignUpPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var signUpTaskSource = new TaskCompletionSource<bool>();
                this.signUpTask = signUpTaskSource.Task;

                var navigationService = new Mock<INavigationService>();
                navigationService
                    .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                    .Callback((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                    {
                        this.navigatePageName = name;
                    });

                this.ViewModel = new SignUpPageViewModel(navigationService.Object).BuildUp(container);
                this.ViewModel.Nickname.Value = "Foo.Bar";

                this.ViewModel.Model.SignUpSuccess += (sender, e) => signUpTaskSource.SetResult(true);
                this.ViewModel.Model.SignUpFail += (sender, e) => signUpTaskSource.SetResult(false);

                this.ViewModel.SignUpCommand.Execute();

                if (this.NeedSignUpWait)
                {
                    this.signUpTask.Wait();
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
            protected override string HttpStabResponse => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\"}";

            [TestCase]
            public void ItShouldSignInSuccess()
            {
                Assert.That(this.signUpTask.Result, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldNavigateToFloorMap()
            {
                Assert.That(this.navigatePageName, Is.EqualTo("/MainPage/NavigationPage/FloorMapPage"));
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
                Assert.That(this.signUpTask.Result, Is.EqualTo(false));
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
    public class SignInCommandTest : ViewModelTestBase
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

            var viewModel = new SignUpPageViewModel(navigationService.Object);

            viewModel.SignInCommand.Execute();

            while (!viewModel.SignInCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToSignUpPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("SignInPage"));
        }
    }
}
