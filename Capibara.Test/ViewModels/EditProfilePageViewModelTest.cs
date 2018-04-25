using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Models;

using Moq;
using NUnit.Framework;
using Prism.Services;

using SubjectViewModel = Capibara.ViewModels.EditProfilePageViewModel;

namespace Capibara.Test.ViewModels.EditProfilePageViewModel
{
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
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
