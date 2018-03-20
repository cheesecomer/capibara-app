﻿using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;
using Capibara.Net;
using Capibara.Net.Rooms;

using Moq;
using NUnit.Framework;

namespace Capibara.Test.ViewModels.RoomPageViewModelTest
{
    [TestFixture]
    public class ShowParticipantsCommandTest : ViewModelTestBase
    {
        [SetUp]
        public void SetUp()
        {
            var viewModel = new RoomPageViewModel(this.NavigationService);

            viewModel.ShowParticipantsCommand.Execute();

            while (!viewModel.ShowParticipantsCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("ParticipantsPage"));
        }

        [TestCase]
        public void ItShouldNavigationParametersHsaModel()
        {
            Assert.That(this.NavigationParameters.ContainsKey(ParameterNames.Model), Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldNavigationParameterModelIsRoom()
        {
            Assert.That(this.NavigationParameters[ParameterNames.Model] is Room, Is.EqualTo(true));
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
                var viewModel = new RoomPageViewModel();
                var container = this.GenerateUnityContainer();
                var model = new Mock<Room>();
                model.SetupGet(x => x.IsConnected).Returns(true);

                viewModel = new RoomPageViewModel(model: model.Object).BuildUp(container);

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
                var viewModel = new RoomPageViewModel();
                var container = this.GenerateUnityContainer();
                var model = new Mock<Room>();
                model.SetupGet(x => x.IsConnected).Returns(false);

                viewModel = new RoomPageViewModel(model: model.Object).BuildUp(container);

                viewModel.Message.Value = message;

                Assert.That(viewModel.SpeakCommand.CanExecute(), Is.EqualTo(expect));
            }
        }
    }

    namespace SpeakCommandExecuteTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            protected RoomPageViewModel viewModel;

            protected bool IsSpeakCalled;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                var model = new Mock<Room>();
                model.SetupGet(x => x.IsConnected).Returns(true);
                model.Setup(x => x.Speak(It.IsAny<string>())).ReturnsAsync(true).Callback(() =>
                {
                    this.IsSpeakCalled = true;
                    model.Raise(x => x.SpeakSuccess += null, System.EventArgs.Empty);
                });

                viewModel = new RoomPageViewModel(model: model.Object).BuildUp(container);

                viewModel.ConnectCommand.Execute();
                while (!viewModel.ConnectCommand.CanExecute()) { }

                viewModel.Message.Value = "Foo.Bar!";
                viewModel.SpeakCommand.Execute();
            }

            [TearDown]
            public void TearDown()
            {
                viewModel.Model.Close().Wait();
            }

            [TestCase]
            public void ItShouldMessageWithEmpty()
            {
                Assert.That(this.viewModel.Message.Value, Is.EqualTo(string.Empty));
            }

            [TestCase]
            public void ItShouldIsSpeakCalled()
            {
                Assert.That(this.IsSpeakCalled, Is.EqualTo(true));
            }
        }
    }

    namespace ConnectCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            protected RoomPageViewModel ViewModel { get; private set; }

            protected bool IsRefreshCalled;

            protected bool IsConnectCalled;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var model = new Mock<Room>();
                model.Setup(x => x.Connect()).ReturnsAsync(true).Callback(() => this.IsConnectCalled = true);
                model.Setup(x => x.Refresh()).ReturnsAsync(true).Callback(() => this.IsRefreshCalled = true);

                ViewModel = new RoomPageViewModel(this.NavigationService, model: model.Object);

                ViewModel.BuildUp(container);

                ViewModel.ConnectCommand.Execute();
                while (!ViewModel.ConnectCommand.CanExecute()) { }
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
            protected RoomPageViewModel ViewModel { get; private set; }

            protected bool IsCloseCalled;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var model = new Mock<Room>();
                model.Setup(x => x.Connect()).ReturnsAsync(true);
                model.Setup(x => x.Refresh()).ReturnsAsync(true);
                model.Setup(x => x.Close()).ReturnsAsync(true).Callback(() => this.IsCloseCalled = true);

                ViewModel = new RoomPageViewModel(this.NavigationService, model: model.Object);

                ViewModel.BuildUp(container);

                ViewModel.ConnectCommand.Execute();
                while (!ViewModel.ConnectCommand.CanExecute()) { }

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
            protected RoomPageViewModel ViewModel { get; private set; }

            protected bool IsCloseCalled;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var model = new Mock<Room>();
                model.Setup(x => x.Connect()).ReturnsAsync(true);
                model.Setup(x => x.Refresh()).ReturnsAsync(true);
                model.Setup(x => x.Close()).ReturnsAsync(true).Callback(() => this.IsCloseCalled = true);

                ViewModel = new RoomPageViewModel(this.NavigationService, model: model.Object);

                ViewModel.BuildUp(container);

                ViewModel.ConnectCommand.Execute();
                while (!ViewModel.ConnectCommand.CanExecute()) { }

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
            protected RoomPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                this.ViewModel = new RoomPageViewModel().BuildUp(this.GenerateUnityContainer());
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
            protected RoomPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                this.ViewModel = new RoomPageViewModel().BuildUp(this.GenerateUnityContainer());
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
            protected RoomPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                this.ViewModel = new RoomPageViewModel().BuildUp(this.GenerateUnityContainer());
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
}
