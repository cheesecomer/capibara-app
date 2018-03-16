using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;

using Prism.Navigation;

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
        public class WhenConnectAfterSetMessage : ViewModelTestBase
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

                viewModel = new RoomPageViewModel().BuildUp(container);

                viewModel.ConnectCommand.Execute();
                while (!viewModel.ConnectCommand.CanExecute()) { }

                viewModel.Message.Value = message;

                Assert.That(viewModel.SpeakCommand.CanExecute(), Is.EqualTo(expect));
            }
        }

        [TestFixture]
        public class WhenConnectBeforeSetMessage : ViewModelTestBase
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

                viewModel = new RoomPageViewModel().BuildUp(container);

                viewModel.Message.Value = message;

                viewModel.ConnectCommand.Execute();
                while (!viewModel.ConnectCommand.CanExecute()) { }

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

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                viewModel = new RoomPageViewModel().BuildUp(container);
                viewModel.Model.Restore(new Room() { Id = 1 });

                var speakTaskSource = new TaskCompletionSource<bool>();
                viewModel.Model.SpeakSuccess += (sender, e) => speakTaskSource.SetResult(true);
                viewModel.Model.SpeakFail += (sender, e) => speakTaskSource.SetResult(false);

                viewModel.ConnectCommand.Execute();
                while (!viewModel.ConnectCommand.CanExecute()) { }

                this.ResetSendAsync();

                viewModel.Message.Value = "Foo.Bar!";
                viewModel.SpeakCommand.Execute();

                speakTaskSource.Task.Wait();
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
        }
    }

    namespace ConnectCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            protected RoomPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                ViewModel = new RoomPageViewModel();
                ViewModel.Model.Restore(new Room() { Id = 1 });

                ViewModel.BuildUp(container);

                ViewModel.ConnectCommand.Execute();
                while (!ViewModel.ConnectCommand.CanExecute()) { }

                // 受信完了を待機
                Task.WaitAll(this.ReceiveMessages.Select(x => x.TaskCompletionSource.Task).ToArray());
            }

            [TearDown]
            public void TearDown()
            {
                this.ViewModel.Model.Close().Wait();
            }

            [TestCase]
            public void ItShouldIsConnected()
            {
                Assert.That(this.ViewModel.Model.IsConnected, Is.EqualTo(true));
            }
        }
    }

    namespace CloseCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            protected RoomPageViewModel ViewModel { get; private set; }

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                ViewModel = new RoomPageViewModel();
                ViewModel.Model.Restore(new Room() { Id = 1 });

                ViewModel.BuildUp(container);

                ViewModel.ConnectCommand.Execute();
                while (!ViewModel.ConnectCommand.CanExecute()) { }

                ViewModel.CloseCommand.Execute();
                while (!ViewModel.CloseCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldIsNotConnected()
            {
                Assert.That(this.ViewModel.Model.IsConnected, Is.EqualTo(false));
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

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                ViewModel = new RoomPageViewModel(this.NavigationService);
                ViewModel.Model.Restore(new Room() { Id = 1 });

                ViewModel.BuildUp(container);

                ViewModel.ConnectCommand.Execute();
                while (!ViewModel.ConnectCommand.CanExecute()) { }

                ViewModel.ShowParticipantsCommand.Execute();
                while (!ViewModel.ShowParticipantsCommand.CanExecute()) { }

                ViewModel.CloseCommand.Execute();
                while (!ViewModel.CloseCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldIsNotConnected()
            {
                Assert.That(this.ViewModel.Model.IsConnected, Is.EqualTo(true));
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
