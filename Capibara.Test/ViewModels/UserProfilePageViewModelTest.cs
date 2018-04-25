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
    public class IsFollowPropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new User { Nickname = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(this.Container);

            this.Subject = new SubjectViewModel(model: model).BuildUp(this.Container);
        }

        [TestCase]
        public void ItShouldValueWithExpect()
        {
            Assert.That(this.Subject.IsFollow.Value, Is.EqualTo(false));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Subject.Model.FollowId = 1;
            Assert.That(this.Subject.IsFollow.Value, Is.EqualTo(true));
        }
    }

    [TestFixture]
    public class BlockCommandCanExecuteTest : ViewModelTestBase
    {
        [TestCase(false, true)]
        [TestCase(true, false)]
        public void ItShouldExpected(bool isBlock, bool canExecute)
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);
            viewModel.IsBlock.Value = isBlock;

            Assert.That(viewModel.BlockCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    [TestFixture]
    public class BlockCommandTest : ViewModelTestBase
    {
        private SubjectViewModel ViewModel;

        protected Mock<User> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();
            this.Model.SetupAllProperties();
            this.Model.Setup(x => x.Block()).ReturnsAsync(true);

            this.ViewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object).BuildUp(this.Container);
            this.ViewModel.IsBlock.Value = false;
            this.ViewModel.BlockCommand.Execute();
        }

        [TestCase]
        public void ItShouldIsBlockCalled()
        {
            this.Model.Verify(x => x.Block(), Times.Once());
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
                model.IsBlock = false;
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

                model.IsBlock = true;

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
            var model = new Mock<User>();
            model.SetupAllProperties();
            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);
            viewModel.IsBlock.Value = isBlock;

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
            this.Model.Setup(x => x.Block()).ReturnsAsync(true);

            this.ViewModel = new SubjectViewModel(this.NavigationService.Object, model: this.Model.Object).BuildUp(this.Container);
            this.ViewModel.IsBlock.Value = false;
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
