using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;
using Prism.Services;
using Prism.Navigation;

using Xamarin.Forms;

using SubjectViewModel = Capibara.ViewModels.UserProfilePageViewModel;

namespace Capibara.Test.ViewModels.UserProfilePageViewModel
{

    namespace ToggleBlockDescriptionTest
    {
        public class WhenIsNotBlock
        {
            [TestCase]
            public void ItShouldExpect()
            {
                var model = new User();

                var viewModel = new SubjectViewModel(model: model);
                model.BlockId = null;

                Assert.That(viewModel.ToggleBlockDescription.Value, Is.EqualTo("ブロックする"));
            }
        }

        public class WhenIsBlock
        {
            [TestCase]
            public void ItShouldExpect()
            {
                var model = new User();

                var viewModel = new SubjectViewModel(model: model);

                model.BlockId = 1;

                Assert.That(viewModel.ToggleBlockDescription.Value, Is.EqualTo("ブロック中"));
            }
        }
    }

    [TestFixture]
    public class ToggleBlockCommandCanExecuteTest : ViewModelTestBase
    {
        [TestCase(false, true)]
        [TestCase(true, true)]
        public void ItShouldExpected(bool isBlock, bool canExecute)
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);
            viewModel.Model.BlockId = isBlock ? (int?)1 : null;

            Assert.That(viewModel.ToggleBlockCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    [TestFixture]
    public class ToggleBlockCommandTest : ViewModelTestBase
    {
        private SubjectViewModel ViewModel;

        protected Mock<User> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();
            this.Model.SetupAllProperties();
            this.Model.Setup(x => x.ToggleBlock()).ReturnsAsync(true);

            this.ViewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object).BuildUp(this.Container);
            this.ViewModel.Model.BlockId = null;
            this.ViewModel.ToggleBlockCommand.Execute();
        }

        [TestCase]
        public void ItShouldIsBlockCalled()
        {
            this.Model.Verify(x => x.ToggleBlock(), Times.Once());
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }
    }

    [TestFixture]
    public class ReportCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldNavigateToEditProfilePage()
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.ReportCommand.Execute();

            while (!viewModel.ReportCommand.CanExecute()) { }

            this.NavigationService.Verify(
                x => x.NavigateAsync(
                    "ReportPage",
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == viewModel.Model)),
                Times.Once());
        }

        private SubjectViewModel ViewModel;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.ViewModel = new SubjectViewModel(this.NavigationService.Object);

            this.ViewModel.ReportCommand.Execute();

            while (!this.ViewModel.ReportCommand.CanExecute()) { }
        }
    }

    namespace ToggleFollowDescriptionTest
    {
        public class WhenIsFollow
        {
            [TestCase]
            public void ItShouldExpect()
            {
                var model = new User();

                var viewModel = new SubjectViewModel(model: model);
                model.FollowId = 1;

                Assert.That(viewModel.ToggleFollowDescription.Value, Is.EqualTo("DM を受け付けています"));
            }
        }

        public class WhenIsNotFollow
        {
            [TestCase]
            public void ItShouldExpect()
            {
                var model = new User();

                var viewModel = new SubjectViewModel(model: model);
                model.BlockId = null;
                model.FollowId = null;

                Assert.That(viewModel.ToggleFollowDescription.Value, Is.EqualTo("DM を受け付ける"));
            }
        }

        public class WhenIsBlock
        {
            [TestCase]
            public void ItShouldExpect()
            {
                var model = new User();

                var viewModel = new SubjectViewModel(model: model);

                model.BlockId = 1;

                Assert.That(viewModel.ToggleFollowDescription.Value, Is.EqualTo("DM を受け付ける"));
            }
        }
    }

    [TestFixture]
    public class ToggleFollowCommandCanExecuteTest : ViewModelTestBase
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void ItShouldExpected(bool isBlock, bool canExecute)
        {
            var viewModel = new SubjectViewModel(this.NavigationService.Object).BuildUp(this.Container);
            viewModel.Model.BlockId = isBlock ? (int?)1 : null;

            Assert.That(viewModel.ToggleFollowCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    [TestFixture]
    public class ToggleFollowCommandTest : ViewModelTestBase
    {
        private SubjectViewModel ViewModel;

        protected Mock<User> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();
            this.Model.SetupAllProperties();
            this.Model.Setup(x => x.ToggleFollow()).ReturnsAsync(true);

            this.ViewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object).BuildUp(this.Container);
            this.ViewModel.ToggleFollowCommand.Execute();
        }

        [TestCase]
        public void ItShouldIsBlockCalled()
        {
            this.Model.Verify(x => x.ToggleFollow(), Times.Once());
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }
    }
}
