using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

using SubjectViewModel = Capibara.ViewModels.RoomPageViewModel;

namespace Capibara.Test.ViewModels.RoomPageViewModel
{
    [TestFixture]
    public class ShowParticipantsCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.ShowParticipantsCommand.Execute();

            while (!viewModel.ShowParticipantsCommand.CanExecute()) { }

            this.NavigationService.Verify(
                x => x.NavigateAsync(
                    "ParticipantsPage",
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == viewModel.Model))
                , Times.Once());
        }
    }

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
                var model = new Mock<Room>();
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
                var model = new Mock<Room>();
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

        protected Mock<Room> Model;

        [TestCase]
        public void ItShouldIsSpeakCalled()
        {
            this.Model = new Mock<Room>();
            this.Model.SetupGet(x => x.IsConnected).Returns(true);
            this.Model.Setup(x => x.Speak(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            viewModel = new SubjectViewModel(model: this.Model.Object).BuildUp(this.Container);

            viewModel.OnResume();

            viewModel.Message.Value = "Foo.Bar!";
            viewModel.SpeakCommand.Execute();

            this.Model.Verify(x => x.Speak("Foo.Bar!", string.Empty), Times.Once());
        }
    }

    [TestFixture]
    public class OnSpeakSuccessTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldMessageWithEmpty()
        {
            var model = new Mock<Room>();
            model.SetupGet(x => x.IsConnected).Returns(true);
            model.Setup(x => x.Speak(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

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

            protected Mock<Room> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Model = new Mock<Room>();
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

            protected Mock<Room> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Model = new Mock<Room>();
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

        [TestFixture]
        public class WhenAfterShowParticipantsCommand : ViewModelTestBase
        {
            protected SubjectViewModel ViewModel { get; private set; }

            protected Mock<Room> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.Model = new Mock<Room>();
                this.Model.Setup(x => x.Connect()).ReturnsAsync(true);
                this.Model.Setup(x => x.Refresh()).ReturnsAsync(true);
                this.Model.Setup(x => x.Close()).ReturnsAsync(true);

                ViewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object);

                ViewModel.BuildUp(this.Container);

                ViewModel.OnResume();

                ViewModel.ShowParticipantsCommand.Execute();
                while (!ViewModel.ShowParticipantsCommand.CanExecute()) { }

                ViewModel.CloseCommand.Execute();
                while (!ViewModel.CloseCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldIsCloseNotCalled()
            {
                this.Model.Verify(x => x.Close(), Times.Never());
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
                this.ViewModel.Model.Name = "AAA";
                Assert.That(this.ViewModel.Name.Value, Is.EqualTo(this.ViewModel.Model.Name));
            }

            [TearDown]
            public void TearDown()
            {
                this.ViewModel.Model.Close().Wait();
            }
        }
    }

    namespace CapacityPropertyTest
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
                this.ViewModel.Model.Capacity += 100;
                Assert.That(this.ViewModel.Capacity.Value, Is.EqualTo(this.ViewModel.Model.Capacity));
            }

            [TearDown]
            public void TearDown()
            {
                this.ViewModel.Model.Close().Wait();
            }
        }
    }

    namespace NumberOfParticipantsPropertyTest
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
                this.ViewModel.Model.NumberOfParticipants += 100;
                Assert.That(this.ViewModel.NumberOfParticipants.Value, Is.EqualTo(this.ViewModel.Model.NumberOfParticipants));
            }

            [TearDown]
            public void TearDown()
            {
                this.ViewModel.Model.Close().Wait();
            }
        }
    }

    [TestFixture]
    public class RejectSubscriptionTest : ViewModelTestBase
    {
        protected SubjectViewModel viewModel;

        protected Mock<Room> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<Room>();

            this.Model.Setup(x => x.Close()).ReturnsAsync(true);

            viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, this.Model.Object).BuildUp(this.Container);

            this.Model.Raise(x => x.RejectSubscription += null, System.EventArgs.Empty);
        }

        [TestCase]
        public void ItShouldCloseCalled()
        {
            this.Model.Verify(x => x.Close(), Times.Once());
        }
    }
}
