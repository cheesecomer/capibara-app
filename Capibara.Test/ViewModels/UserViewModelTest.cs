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

using SubjectViewModel = Capibara.ViewModels.UserViewModel;

namespace Capibara.Test.ViewModels.UserViewModelTest
{
    public class NicknamePropertyTest : ViewModelTestBase
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
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx"));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Subject.Model.Nickname = "xxxx!!!";
            Assert.That(this.Subject.Nickname.Value, Is.EqualTo("xxxx!!!"));
        }
    }

    public class BiographyPropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var container = this.Container;
            var model = new User() { Biography = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(this.Container);
            this.Subject = new SubjectViewModel(model: model).BuildUp(this.Container);
        }

        [TestCase]
        public void ItShouldValueWithExpect()
        {
            Assert.That(this.Subject.Biography.Value, Is.EqualTo("xxxx"));
        }

        [TestCase]
        public void ItShouldUpdate()
        {
            this.Subject.Model.Biography = "xxxx!!!";
            Assert.That(this.Subject.Biography.Value, Is.EqualTo("xxxx!!!"));
        }
    }

    [TestFixture]
    public class RefreshCommandTest : ViewModelTestBase
    {
        private bool IsRefreshCalled;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Refresh()).ReturnsAsync(true).Callback(() => this.IsRefreshCalled = true);

            var viewModel = new SubjectViewModel(model: model.Object).BuildUp(this.Container);

            viewModel.RefreshCommand.Execute();

            while (!viewModel.RefreshCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }

        [TestCase]
        public void ItShouldRefreshCalled()
        {
            Assert.That(this.IsRefreshCalled, Is.EqualTo(true));
        }
    }

    namespace ChangePhotoCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            private ActionSheetButton[] buttons;

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

                var viewModel = new SubjectViewModel(pageDialogService: this.PageDialogService.Object).BuildUp(this.Container);
                viewModel.Model.Id = 1;

                viewModel.ChangePhotoCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldHasFourButtons()
            {
                Assert.That(this.buttons?.Length, Is.EqualTo(2));
            }

            [TestCase(0, "キャンセル")]
            [TestCase(1, "アルバムから選択")]
            public void ItShouldButtontTextExpected(int index, string expect)
            {
                Assert.That(this.buttons.ElementAtOrDefault(index).Text, Is.EqualTo(expect));
            }
        }

        [TestFixture]
        public class WhenPickupSuccess : ViewModelTestBase
        {
            private ActionSheetButton[] buttons;

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

                viewModel.ChangePhotoCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { }

                this.buttons.ElementAtOrDefault(1)?.Action?.Invoke();
            }

            [TestCase]
            public void ItShouldShowPhotoPicker()
            {
                this.PickupPhotoService.Verify(x => x.DisplayAlbumAsync(), Times.Once());
            }
        }
    }

    [TestFixture]
    public class EditCommandTest : ViewModelTestBase
    {
        [TestCase]
        public void ItShouldNavigateToEditProfilePage()
        {

            var viewModel = new SubjectViewModel(this.NavigationService.Object);

            viewModel.EditCommand.Execute();

            while (!viewModel.EditCommand.CanExecute()) { }

            this.NavigationService.Verify(
                x => x.NavigateAsync(
                    "EditProfilePage",
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == viewModel.Model))
                , Times.Once());
        }
    }

    [TestFixture]
    public class CommitCommandCanExecuteTest : ViewModelTestBase
    {
        [TestCase("", false)]
        [TestCase(" ", false)]
        [TestCase("a", true)]
        [TestCase(" a ", true)]
        public void ItShouldExpected(string nickname, bool canExecute)
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);
            viewModel.Nickname.Value = nickname;

            Assert.That(viewModel.CommitCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    [TestFixture]
    public class CommitCommandTest : ViewModelTestBase
    {
        private bool IsCommitCalled;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Commit()).ReturnsAsync(true).Callback(() => this.IsCommitCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);
            viewModel.Nickname.Value = "FooBar";

            viewModel.CommitCommand.Execute();

            while (!viewModel.CommitCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
        }

        [TestCase]
        public void ItShouldCommitCalled()
        {
            Assert.That(this.IsCommitCalled, Is.EqualTo(true));
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

        protected bool IsBlockCalled;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Block()).ReturnsAsync(true).Callback(() => this.IsBlockCalled = true);

            this.ViewModel = new SubjectViewModel(this.NavigationService.Object, model: model.Object).BuildUp(this.Container);
            this.ViewModel.IsBlock.Value = false;
            this.ViewModel.BlockCommand.Execute();
        }

        [TestCase]
        public void ItShouldIsBlockCalled()
        {
            Assert.That(this.IsBlockCalled, Is.EqualTo(true));
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
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == viewModel.Model))
                , Times.Once());
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
}
