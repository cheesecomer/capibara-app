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
    namespace OnResumeTest
    {
        public class WhenOther : ViewModelTestBase
        {
            public void ItShouldSendView()
            {
                this.IsolatedStorage.UserId = 2;

                var subject = new SubjectViewModel(model: new User { Id = 1 }).BuildUp(this.Container);

                subject.OnResume();

                this.Tracker.Verify(x => x.SendView(It.Is<string>(v => v == "/User/1")), Times.Once());
            }
        }

        public class WhenOwn : ViewModelTestBase
        {
            public void ItShouldSendView()
            {
                this.IsolatedStorage.UserId = 1;

                var subject = new SubjectViewModel(model: new User { Id = 1 }).BuildUp(this.Container);

                subject.OnResume();

                this.Tracker.Verify(x => x.SendView(It.IsAny<string>()), Times.Never());
            }
        }
    }

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
        private Mock<User> Model;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.Model = new Mock<User>();
            this.Model.SetupAllProperties();
            this.Model.Setup(x => x.Refresh()).ReturnsAsync(true);

            var viewModel = new SubjectViewModel(model: this.Model.Object).BuildUp(this.Container);

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
            this.Model.Verify(x => x.Refresh(), Times.Once());
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
                this.PickupPhotoService.Verify(x => x.DisplayAlbumAsync(Services.CropMode.Square), Times.Once());
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
                    It.Is<NavigationParameters>(v => v.GetValueOrDefault(ParameterNames.Model) == viewModel.Model)),
                Times.Once());
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

    namespace CommitCommandTest
    {
        [TestFixture]
        public class WhenIconBase64IsEmpty : ViewModelTestBase
        {
            protected Mock<User> Model;
            
            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.PageDialogService.Reset();
                this.PageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(true);

                this.RewardedVideoService
                    .Setup(x => x.DisplayRewardedVideo())
                    .ReturnsAsync(true);
                
                this.Model = new Mock<User>();
                this.Model.SetupAllProperties();
                this.Model.Setup(x => x.Commit()).ReturnsAsync(true);
                
                var viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, this.Model.Object).BuildUp(this.Container);
                viewModel.Nickname.Value = "FooBar";
                
                viewModel.CommitCommand.Execute();
                
                while (!viewModel.CommitCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldNotShowDialog()
            {
                this.PageDialogService.Verify(
                    x => x.DisplayAlertAsync(
                        string.Empty,
                        "動画広告を視聴して\r\nプロフィール画像を更新しよう！", 
                        "視聴する", 
                        "閉じる"), 
                    Times.Never());
            }

            [TestCase]
            public void ItShouldNotShowRewardVideo()
            {
                this.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Never());
            }
            
            [TestCase]
            public void ItShouldShowProgressDialog()
            {
                this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
            }
            
            [TestCase]
            public void ItShouldCommitCalled()
            {
                this.Model.Verify(x => x.Commit(), Times.Once());
            }
        }

        [TestFixture]
        public class WhenIconBase64IsPresent : ViewModelTestBase
        {
            protected Mock<User> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                //this.PageDialogService.Reset();
                this.PageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),It.IsAny<string>()))
                    .ReturnsAsync(true);

                this.RewardedVideoService
                    .Setup(x => x.DisplayRewardedVideo())
                    .ReturnsAsync(true);

                this.Model = new Mock<User>();
                this.Model.SetupGet(x => x.IconBase64).Returns("abcdefg");
                this.Model.SetupGet(x => x.Nickname).Returns("FooBar");
                this.Model.Setup(x => x.Commit()).ReturnsAsync(true);

                var viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, this.Model.Object).BuildUp(this.Container);

                viewModel.CommitCommand.Execute();

                while (!viewModel.CommitCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldNotShowDialog()
            {
                this.PageDialogService
                    .Verify(
                        x => x.DisplayAlertAsync(
                            string.Empty,
                            "動画広告を視聴して\r\nプロフィール画像を更新しよう！",
                            "視聴する",
                            "閉じる"),
                        Times.Once());
            }

            [TestCase]
            public void ItShouldNotShowRewardVideo()
            {
                this.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Once());
            }

            [TestCase]
            public void ItShouldShowProgressDialog()
            {
                this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()));
            }

            [TestCase]
            public void ItShouldCommitCalled()
            {
                this.Model.Verify(x => x.Commit(), Times.Once());
            }
        }

        [TestFixture]
        public class WhenIconBase64IsPresentDialogCancel : ViewModelTestBase
        {
            protected Mock<User> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.PageDialogService.Reset();
                this.PageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(false);

                this.Model = new Mock<User>();
                this.Model.SetupGet(x => x.IconBase64).Returns("abcdefg");
                this.Model.SetupGet(x => x.Nickname).Returns("FooBar");
                this.Model.Setup(x => x.Commit()).ReturnsAsync(true);

                var viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, this.Model.Object).BuildUp(this.Container);

                viewModel.CommitCommand.Execute();

                while (!viewModel.CommitCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldNotShowDialog()
            {
                this.PageDialogService
                    .Verify(
                        x => x.DisplayAlertAsync(
                            string.Empty,
                            "動画広告を視聴して\r\nプロフィール画像を更新しよう！",
                            "視聴する",
                            "閉じる"),
                        Times.Once());
            }

            [TestCase]
            public void ItShouldNotShowRewardVideo()
            {
                this.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Never());
            }

            [TestCase]
            public void ItShouldShowProgressDialog()
            {
                this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()), Times.Never());
            }

            [TestCase]
            public void ItShouldCommitCalled()
            {
                this.Model.Verify(x => x.Commit(), Times.Never());
            }
        }

        [TestFixture]
        public class WhenIconBase64IsPresentRewardVideoCancel : ViewModelTestBase
        {
            protected Mock<User> Model;

            [SetUp]
            public override void SetUp()
            {
                base.SetUp();

                this.PageDialogService.Reset();
                this.PageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .ReturnsAsync(true);

                this.Model = new Mock<User>();
                this.Model.SetupGet(x => x.IconBase64).Returns("abcdefg");
                this.Model.SetupGet(x => x.Nickname).Returns("FooBar");
                this.Model.Setup(x => x.Commit()).ReturnsAsync(true);

                var viewModel = new SubjectViewModel(this.NavigationService.Object, this.PageDialogService.Object, this.Model.Object).BuildUp(this.Container);

                viewModel.CommitCommand.Execute();

                while (!viewModel.CommitCommand.CanExecute()) { }
            }

            [TestCase]
            public void ItShouldNotShowDialog()
            {
                this.PageDialogService
                    .Verify(
                        x => x.DisplayAlertAsync(
                            string.Empty,
                            "動画広告を視聴して\r\nプロフィール画像を更新しよう！",
                            "視聴する",
                            "閉じる"),
                        Times.Once());
            }

            [TestCase]
            public void ItShouldNotShowRewardVideo()
            {
                this.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Once());
            }

            [TestCase]
            public void ItShouldShowProgressDialog()
            {
                this.ProgressDialogService.Verify(x => x.DisplayProgressAsync(It.IsAny<Task>(), It.IsAny<string>()), Times.Never());
            }

            [TestCase]
            public void ItShouldCommitCalled()
            {
                this.Model.Verify(x => x.Commit(), Times.Never());
            }
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

    [TestFixture]
    public class CooperationSnsCommandTest : ViewModelTestBase
    {
        private ActionSheetButton[] buttons;

        [SetUp]
        public override void SetUp()
        {
            base.SetUp();

            this.IsolatedStorage.AccessToken = "abcdefg";

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

            viewModel.CooperationSnsCommand.Execute();

            while (!viewModel.CooperationSnsCommand.CanExecute()) { }
        }

        [TestCase]
        public void ItShouldHasFourButtons()
        {
            Assert.That(this.buttons?.Length, Is.EqualTo(4));
        }

        [TestCase(0, "キャンセル")]
        [TestCase(1, "Google")]
        [TestCase(2, "Twitter")]
        [TestCase(3, "LINE")]
        public void ItShouldButtontTextExpected(int index, string expect)
        {
            Assert.That(this.buttons.ElementAtOrDefault(index).Text, Is.EqualTo(expect));
        }

        [TestCase(1, "http://localhost:9999/api/oauth/google?user_id=1&access_token=abcdefg")]
        [TestCase(2, "http://localhost:9999/api/oauth/twitter?user_id=1&access_token=abcdefg")]
        [TestCase(3, "http://localhost:9999/api/oauth/line?user_id=1&access_token=abcdefg")]
        public void ItShouldOpenUrl(int index, string url)
        {
            this.buttons.ElementAtOrDefault(index)?.Action?.Invoke();
            this.SnsLoginService.Verify(x => x.Open(It.Is<string>(v => new Uri(v).AbsoluteUri == url)), Times.Once());
        }
    }
}
