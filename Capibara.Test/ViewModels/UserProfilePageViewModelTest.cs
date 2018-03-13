using System;
using System.Net;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.ViewModels;

using Microsoft.Practices.Unity;

using Moq;
using NUnit.Framework;

using Prism.Navigation;
using Prism.Services;

namespace Capibara.Test.ViewModels.UserProfilePageViewModelTest
{
    namespace NicknamePropertyTest
    {
        public class WhenOwn : ViewModelTestBase
        {
            protected User Model;

            protected UserProfilePageViewModel Actual;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                this.Model = new User() { Nickname = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(container);
                this.IsolatedStorage.UserId = this.Model.Id;

                this.Actual = new UserProfilePageViewModel(model: this.Model).BuildUp(container);
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.Actual.Nickname.Value, Is.EqualTo("xxxx"));
            }

            [TestCase]
            public void ItShouldUpdate()
            {
                this.Actual.Model.Nickname = "xxxx!!!";
                Assert.That(this.Actual.Nickname.Value, Is.EqualTo("xxxx!!!"));
            }
        }
    }

    namespace BiographyPropertyTest
    {
        public class WhenOwn : ViewModelTestBase
        {
            protected User Model;

            protected UserProfilePageViewModel Actual;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                this.Model = new User() { Biography = "xxxx", Id = Guid.NewGuid().ToInt() }.BuildUp(container);
                this.IsolatedStorage.UserId = this.Model.Id;

                this.Actual = new UserProfilePageViewModel(model: this.Model).BuildUp(container);
            }

            [TestCase]
            public void ItShouldValueWithExpect()
            {
                Assert.That(this.Actual.Biography.Value, Is.EqualTo("xxxx"));
            }

            [TestCase]
            public void ItShouldUpdate()
            {
                this.Actual.Model.Biography = "xxxx!!!";
                Assert.That(this.Actual.Biography.Value, Is.EqualTo("xxxx!!!"));
            }
        }
    }

    namespace RefreshCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();

                var viewModel = new UserProfilePageViewModel();

                viewModel.Model.Id = 1;

                viewModel.BuildUp(container);

                viewModel.RefreshCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { };
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
            }
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

                var viewModel = new UserProfilePageViewModel(pageDialogService: pageDialogService.Object);

                viewModel.Model.Id = 1;

                viewModel.BuildUp(container);

                viewModel.ChangePhotoCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { };
            }

            [TestCase]
            public void ItShouldHasFourButtons()
            {
                Assert.That(this.buttons?.Length, Is.EqualTo(4));
            }

            [TestCase]
            public void ItShouldCancelIsFirstButton()
            {
                Assert.That(this.buttons.ElementAtOrDefault(0).Text, Is.EqualTo("キャンセル"));
            }

            [TestCase]
            public void ItShouldDeleteIsSecondButton()
            {
                Assert.That(this.buttons.ElementAtOrDefault(1).Text, Is.EqualTo("削除"));
            }

            [TestCase]
            public void ItShouldPickupIsThaadButton()
            {
                Assert.That(this.buttons.ElementAtOrDefault(2).Text, Is.EqualTo("アルバムから選択"));
            }

            [TestCase]
            public void ItShouldTakeIsForthButton()
            {
                Assert.That(this.buttons.ElementAtOrDefault(3).Text, Is.EqualTo("カメラで撮影"));
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

                var viewModel = new UserProfilePageViewModel(pageDialogService: pageDialogService.Object);

                viewModel.Model.Id = 1;

                viewModel.BuildUp(container);

                viewModel.ChangePhotoCommand.Execute();

                while (!viewModel.RefreshCommand.CanExecute()) { };

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
        protected string NavigatePageName { get; private set; }

        protected NavigationParameters NavigationParameters { get; private set; }

        [SetUp]
        public void SetUp()
        {
            var navigationService = new Mock<INavigationService>();
            navigationService
                .Setup(x => x.NavigateAsync(It.IsAny<string>(), It.IsAny<NavigationParameters>(), It.IsAny<bool?>(), It.IsAny<bool>()))
                .Returns((string name, NavigationParameters parameters, bool? useModalNavigation, bool animated) =>
                {
                    this.NavigatePageName = name;
                    this.NavigationParameters = parameters;
                    return Task.Run(() => { });
                });

            var viewModel = new UserProfilePageViewModel(navigationService.Object);

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

    namespace CommitCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                var navigationService = new Mock<INavigationService>();
                var viewModel = new UserProfilePageViewModel(navigationService.Object);

                viewModel.Model.Id = 1;

                viewModel.BuildUp(container);

                viewModel.CommitCommand.Execute();

                while (!viewModel.CommitCommand.CanExecute()) { };
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
            }
        }
    }

    namespace BlockCommandTest
    {
        [TestFixture]
        public class WhenSuccess : ViewModelTestBase
        {
            private UserProfilePageViewModel ViewModel;

            [SetUp]
            public void SetUp()
            {
                var container = this.GenerateUnityContainer();
                var navigationService = new Mock<INavigationService>();

                this.ViewModel = new UserProfilePageViewModel(navigationService.Object);

                this.ViewModel.Model.Id = 1;

                this.ViewModel.BuildUp(container);

                var blockTaskSource = new TaskCompletionSource<bool>();
                this.ViewModel.Model.BlockSuccess += (sender, e) => blockTaskSource.SetResult(true);
                this.ViewModel.Model.BlockFail += (sender, e) => blockTaskSource.SetResult(false);

                this.ViewModel.BlockCommand.Execute();

                blockTaskSource.Task.Wait();
            }

            [TestCase]
            public void ItShouldShowDialog()
            {
                Assert.That(this.IsDisplayedProgressDialog, Is.EqualTo(true));
            }

            [TestCase]
            public void ItShouldCanNotExecute()
            {
                Assert.That(this.ViewModel.BlockCommand.CanExecute(), Is.EqualTo(false));
            }

            [TestCase]
            public void ItShouldIsBlocked()
            {
                Assert.That(this.ViewModel.IsBlock.Value, Is.EqualTo(true));
            }
        }
    }
}
