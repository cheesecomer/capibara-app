using System;
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
            protected override string HttpStabResponse => "{ \"access_token\": \"1:bGbDyyVxbSQorRhgyt6R\"}";

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
}
