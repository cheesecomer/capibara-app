using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Moq;
using NUnit.Framework;
using Prism.Services;

using SubjectViewModel = Capibara.ViewModels.UserViewModel;

namespace Capibara.Test.ViewModels.UserViewModelTest
{
    public class NicknamePropertyTest : ViewModelTestBase
    {
        protected SubjectViewModel Subject;

        [SetUp]
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();
            var model = new User { Nickname = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(container);

            this.Subject = new SubjectViewModel(model: model).BuildUp(container);
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
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();
            var model = new User() { Biography = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(container);
            this.Subject = new SubjectViewModel(model: model).BuildUp(container);
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
        public void SetUp()
        {
            var container = this.GenerateUnityContainer();

            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Refresh()).ReturnsAsync(true).Callback(() => this.IsRefreshCalled = true);

            var viewModel = new SubjectViewModel(model: model.Object).BuildUp(this.GenerateUnityContainer());

            viewModel.RefreshCommand.Execute();

            while (!viewModel.RefreshCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
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
            public void SetUp()
            {
                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                    .Returns((string name, IActionSheetButton[] buttons) =>
                    {
                        this.buttons = buttons.Select(x => x as ActionSheetButton).ToArray();
                        return Task.Run(() => { });
                    });

                var container = this.GenerateUnityContainer();

                var viewModel = new SubjectViewModel(pageDialogService: pageDialogService.Object);

                viewModel.Model.Id = 1;

                viewModel.BuildUp(container);

                viewModel.ChangePhotoCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldHasFourButtons()
            {
                Assert.That(this.buttons?.Length, Is.EqualTo(4));
            }

            [TestCase(0, "キャンセル")]
            [TestCase(1, "削除")]
            [TestCase(2, "アルバムから選択")]
            [TestCase(3, "カメラで撮影")]
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
            public void SetUp()
            {
                var pageDialogService = new Mock<IPageDialogService>();
                pageDialogService
                    .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                    .Returns((string name, IActionSheetButton[] buttons) =>
                    {
                        this.buttons = buttons.Select(x => x as ActionSheetButton).ToArray();
                        return Task.Run(() => { });
                    });

                var container = this.GenerateUnityContainer();

                var viewModel = new SubjectViewModel(pageDialogService: pageDialogService.Object);

                viewModel.Model.Id = 1;

                viewModel.BuildUp(container);

                viewModel.ChangePhotoCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { }

                this.buttons.ElementAtOrDefault(2)?.Action?.Invoke();
            }

            [TestCase]
            public void ItShouldShowPhotoPicker()
            {
                Assert.That(this.IsDisplayedPhotoPicker, Is.EqualTo(true));
            }
        }
    }

    [TestFixture]
    public class EditCommandTest : ViewModelTestBase
    {
        [SetUp]
        public void SetUp()
        {
            var viewModel = new SubjectViewModel(this.NavigationService);

            viewModel.EditCommand.Execute();

            while (!viewModel.EditCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("EditProfilePage"));
        }

        [TestCase]
        public void ItShouldNavigationParametersHsaModel()
        {
            Assert.That(this.NavigationParameters.ContainsKey(ParameterNames.Model), Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldNavigationParameterModelIsUser()
        {
            Assert.That(this.NavigationParameters[ParameterNames.Model] is User, Is.EqualTo(true));
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
            var viewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());
            viewModel.Nickname.Value = nickname;

            Assert.That(viewModel.CommitCommand.CanExecute(), Is.EqualTo(canExecute));
        }
    }

    [TestFixture]
    public class CommitCommandTest : ViewModelTestBase
    {
        private bool IsCommitCalled;

        [SetUp]
        public void SetUp()
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Commit()).ReturnsAsync(true).Callback(() => this.IsCommitCalled = true);

            var viewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());
            viewModel.Nickname.Value = "FooBar";

            viewModel.CommitCommand.Execute();

            while (!viewModel.CommitCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldShowDialog()
        {
            Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
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
            var viewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());
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
        public void SetUp()
        {
            var model = new Mock<User>();
            model.SetupAllProperties();
            model.Setup(x => x.Block()).ReturnsAsync(true).Callback(() => this.IsBlockCalled = true);

            this.ViewModel = new SubjectViewModel(this.NavigationService, model: model.Object).BuildUp(this.GenerateUnityContainer());
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
            Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
        }
    }


    [TestFixture]
    public class ReportCommandTest : ViewModelTestBase
    {
        private SubjectViewModel ViewModel;

        [SetUp]
        public void SetUp()
        {
            this.ViewModel = new SubjectViewModel(this.NavigationService);

            this.ViewModel.ReportCommand.Execute();

            while (!this.ViewModel.ReportCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldNavigateToParticipantsPage()
        {
            Assert.That(this.NavigatePageName, Is.EqualTo("ReportPage"));
        }

        [TestCase]
        public void ItShouldNavigationParametersHsaModel()
        {
            Assert.That(this.NavigationParameters.ContainsKey(ParameterNames.Model), Is.EqualTo(true));
        }

        [TestCase]
        public void ItShouldNavigationParameterModelIsExpect()
        {
            Assert.That(this.NavigationParameters[ParameterNames.Model], Is.EqualTo(this.ViewModel.Model));
        }
    }
}
