﻿using Capibara.Models;
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
            var model = new Mock<Room>();
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

            protected bool IsRefreshCalled;

            protected bool IsConnectCalled;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<Room>();
                model.Setup(x => x.Connect()).ReturnsAsync(true).Callback(() => this.IsConnectCalled = true);
                model.Setup(x => x.Refresh()).ReturnsAsync(true).Callback(() => this.IsRefreshCalled = true);

                ViewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object);

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
                Assert.That(this.IsRefreshCalled, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldIsConnectCalled()
            {
                Assert.That(this.IsConnectCalled, Is.EqualTo(true));
            }
        }
    }

    namespace CloseCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            protected SubjectViewModel ViewModel { get; private set; }

            protected bool IsCloseCalled;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<Room>();
                model.Setup(x => x.Connect()).ReturnsAsync(true);
                model.Setup(x => x.Refresh()).ReturnsAsync(true);
                model.Setup(x => x.Close()).ReturnsAsync(true).Callback(() => this.IsCloseCalled = true);

                ViewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object);

                ViewModel.BuildUp(this.Container);

                ViewModel.OnResume();

                ViewModel.CloseCommand.Execute();
                while (!ViewModel.CloseCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldCloseCalled()
            {
                Assert.That(this.IsCloseCalled, Is.EqualTo(true));
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

            protected bool IsCloseCalled;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                var model = new Mock<Room>();
                model.Setup(x => x.Connect()).ReturnsAsync(true);
                model.Setup(x => x.Refresh()).ReturnsAsync(true);
                model.Setup(x => x.Close()).ReturnsAsync(true).Callback(() => this.IsCloseCalled = true);

                ViewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object);

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
                Assert.That(this.IsCloseCalled, Is.EqualTo(false));
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

        protected bool IsCloseCalled;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<Room>();

            model.Setup(x => x.Close()).Callback(() => this.IsCloseCalled = true).Returns(System.Threading.Tasks.Task.Run(() => true));

            viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, model.Object).BuildUp(this.Container);

            model.Raise(x => x.RejectSubscription += null, System.EventArgs.Empty);
        }

        [TestCase]
        public void ItShouldCloseCalled()
        {
            Assert.That(this.IsCloseCalled, Is.EqualTo(true));
        }
    }
}
