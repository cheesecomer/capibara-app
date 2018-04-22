using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

using SubjectViewModel = Capibara.ViewModels.DirectMessagePageViewModel;

namespace Capibara.Test.ViewModels.DirectMessagePageViewModel
{
    namespace SpeakCommandCanExecuteTest
    {
        [TestFixture]
        public class WhenConnected : ViewModelTestBase
        {
            [TestCase("", false)]
            [TestCase(null, false)]
            [TestCase("    ", false)]
            [TestCase("\r\n", false)]
            [TestCase("  \r\n  ", false)]
            [TestCase("1", true)]
            [TestCase("1  ", true)]
            [TestCase("1 \r\n ", true)]
            public void ItShouldResultWithExpect(string message, bool expect)
            {
                var viewModel = new SubjectViewModel();
                var container = this.Container;
                var model = new Mock<DirectMessageThread>();
                model.SetupGet(x => x.IsConnected).Returns(true);

                viewModel = new SubjectViewModel(model: model.Object).BuildUp(container);

                viewModel.Message.Value = message;

                Assert.That(viewModel.SpeakCommand.CanExecute(), Is.EqualTo(expect));
            }
        }

        [TestFixture]
        public class WhenNotConnected : ViewModelTestBase
        {
            [TestCase("", false)]
            [TestCase(null, false)]
            [TestCase("    ", false)]
            [TestCase("\r\n", false)]
            [TestCase("  \r\n  ", false)]
            [TestCase("1", false)]
            [TestCase("1  ", false)]
            [TestCase("1 \r\n ", false)]
            public void ItShouldResultWithExpect(string message, bool expect)
            {
                var viewModel = new SubjectViewModel();
                var container = this.Container;
                var model = new Mock<DirectMessageThread>();
                model.SetupGet(x => x.IsConnected).Returns(false);

                viewModel = new SubjectViewModel(model: model.Object).BuildUp(container);

                viewModel.Message.Value = message;

                Assert.That(viewModel.SpeakCommand.CanExecute(), Is.EqualTo(expect));
            }
        }
    }
    [TestFixture]
    public class SpeakCommandExecuteTest : ViewModelTestBase
    {
        protected SubjectViewModel viewModel;

        protected Mock<DirectMessageThread> Model;

        [TestCase]
        public void ItShouldIsSpeakCalled()
        {
            this.Model = new Mock<DirectMessageThread>();
            this.Model.SetupGet(x => x.IsConnected).Returns(true);
            this.Model.Setup(x => x.Speak(It.IsAny<string>())).ReturnsAsync(true);

            viewModel = new SubjectViewModel(model: this.Model.Object).BuildUp(this.Container);

            viewModel.OnResume();

            viewModel.Message.Value = "Foo.Bar!";
            viewModel.SpeakCommand.Execute();

            this.Model.Verify(x => x.Speak("Foo.Bar!"), Times.Once());
        }
    }

    [TestFixture]
    public class OnSpeakSuccessTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldMessageWithEmpty()
        {
            var model = new Mock<DirectMessageThread>();
            model.SetupGet(x => x.IsConnected).Returns(true);
            model.Setup(x => x.Speak(It.IsAny<string>())).ReturnsAsync(true);

            var viewModel = new SubjectViewModel(model: model.Object).BuildUp(this.Container);

            viewModel.OnResume();

            viewModel.Message.Value = "Foo.Bar!";
            viewModel.SpeakCommand.Execute();

            model.Raise(x => x.SpeakSuccess += null, System.EventArgs.Empty);
            Assert.That(viewModel.Message.Value, Is.EqualTo(string.Empty));
        }
    }

    namespace OnResumeTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            protected SubjectViewModel ViewModel { get; private set; }

            protected Mock<DirectMessageThread> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Model = new Mock<DirectMessageThread>();
                this.Model.Setup(x => x.Connect()).ReturnsAsync(true);
                this.Model.Setup(x => x.Refresh()).ReturnsAsync(true);

                ViewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object);

                ViewModel.BuildUp(this.Container);

                ViewModel.OnResume();
            }

            [TearDown]
            public void TearDown()
            {
                this.ViewModel.Model.Close().Wait();
            }

            [TestCase]
            public void ItShouldIsRefreshCalled()
            {
                this.Model.Verify(x => x.Refresh(), Times.Once());
            }

            [TestCase]
            public void ItShouldIsConnectCalled()
            {
                this.Model.Verify(x => x.Connect(), Times.Once());
            }
        }
    }

    namespace CloseCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            protected SubjectViewModel ViewModel { get; private set; }

            protected Mock<DirectMessageThread> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Model = new Mock<DirectMessageThread>();
                this.Model.Setup(x => x.Connect()).ReturnsAsync(true);
                this.Model.Setup(x => x.Refresh()).ReturnsAsync(true);
                this.Model.Setup(x => x.Close()).ReturnsAsync(true);

                ViewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object);

                ViewModel.BuildUp(this.Container);

                ViewModel.OnResume();

                ViewModel.CloseCommand.Execute();
                while (!ViewModel.CloseCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldCloseCalled()
            {
                this.Model.Verify(x => x.Close(), Times.Once());
            }

            [TearDown]
            public void TearDown()
            {
                this.ViewModel.Model.Close().Wait();
            }
        }
    }

    namespace NamePropertyTest
    {
        [TestFixture]
        public class WhenUpdate : ViewModelTestBase
        {
            protected SubjectViewModel ViewModel { get; private set; }

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.ViewModel = new SubjectViewModel().BuildUp(this.Container);
            }

            [TestCase]
            public void ItShouldValueToExpect()
            {
                this.ViewModel.Model.User.Nickname = "AAA";
                Assert.That(this.ViewModel.Name.Value, Is.EqualTo("AAA"));
            }
        }
    }
}
