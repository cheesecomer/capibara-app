#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using System.Reactive;
using System.Threading.Tasks;
using System.Linq;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Capibara.Services;
using Moq;
using NUnit.Framework;
using Prism.Services;
using Prism.Navigation;
using Microsoft.Reactive.Testing;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class EditProfilePageViewModelTest
    {
        #region CommitCommand

        public static IEnumerable CommitCommand_CanExecute_TestCaseSource()
        {
            yield return
                new TestCaseData(null, false).SetName("CommitCommand.CanExecute When nickname null should can not execute");
            yield return
                new TestCaseData("", false).SetName("CommitCommand.CanExecute When nickname empty should can not execute");
            yield return
                new TestCaseData(Faker.Name.FullName(), true).SetName("CommitCommand.CanExecute When nickname present should can execute");
        }

        [Test]
        [TestCaseSource("CommitCommand_CanExecute_TestCaseSource")]
        public void CommitCommand_CanExecute(string nickname, bool expected)
        {
            var model = ModelFixture.User();
            model.Nickname = nickname;
            var subject = new EditProfilePageViewModel(model: model);

            Assert.That(subject.CommitCommand.CanExecute(), Is.EqualTo(expected));
        }

        public class CommitCommandSubject
        {
            public TestScheduler Scheduler { get; private set; }

            public User Model { get; private set; }

            public EditProfilePageViewModel ViewModel { get; private set; }

            public Mock<IUpdateProfileUseCase> UpdateProfileUseCase { get; private set; }

            public Mock<IRewardedVideoService> RewardedVideoService { get; private set; }

            public Mock<IPageDialogService> PageDialogService { get; private set; }

            public Mock<IProgressDialogService> ProgressDialogService { get; private set; }

            public Mock<NavigationService> NavigationService { get; private set; }

            public CommitCommandSubject(bool? alertResult = null)
            {
                var schedulerProvider = new SchedulerProvider();
                var scheduler = schedulerProvider.Scheduler;
                var navigationService = Mock.NavigationService();
                var pageDialogService = Mock.PageDialogService(alertResult);
                var progressDialogService = Mock.ProgressDialogService<Unit>();
                var rewardedVideoService = new Mock<IRewardedVideoService>();
                var useCase = new Mock<IUpdateProfileUseCase>();
                var model = ModelFixture.User();
                var viewModel = new EditProfilePageViewModel(
                    model: model,
                    navigationService: navigationService.Object,
                    pageDialogService: pageDialogService.Object)
                {
                    UpdateProfileUseCase = useCase.Object,
                    SchedulerProvider = schedulerProvider,
                    ProgressDialogService = progressDialogService.Object,
                    RewardedVideoService = rewardedVideoService.Object
                };

                this.Scheduler = scheduler;
                this.Model = model;
                this.ViewModel = viewModel;
                this.UpdateProfileUseCase = useCase;
                this.RewardedVideoService = rewardedVideoService;
                this.PageDialogService = pageDialogService;
                this.ProgressDialogService = progressDialogService;
                this.NavigationService = navigationService;
            }
        }

        public static IEnumerable CommitCommand_WhenNotImageChanged_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    1,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never))
                    )
                .SetName("CommitCommand when image not change should show alert");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Once))
                    )
                .SetName("CommitCommand when image not change should invoke usecase");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.NavigationService.Verify(x => x.GoBackAsync(It.Is<NavigationParameters>(
                            parameters =>
                                parameters[ParameterNames.Model] == subject.Model
                            )), Times.Once))
                    )
                .SetName("CommitCommand when image not change should go back with model");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        Assert.True(subject.ViewModel.CommitCommand.CanExecute()))
                    )
                .SetName("CommitCommand when image not change should command can execute");
        }

        [Test]
        [TestCaseSource("CommitCommand_WhenNotImageChanged_TestCaseSource")]
        public void CommitCommand_WhenNotImageChanged(long advanceTime, Action<CommitCommandSubject> assert)
        {
            var subject = new CommitCommandSubject(alertResult: false);

            subject.Model.IconBase64 = null;

            subject.UpdateProfileUseCase.Setup(x => x.Invoke(It.IsAny<User>())).ReturnsObservable();

            subject.RewardedVideoService.Setup(x => x.DisplayRewardedVideo()).ReturnsObservable(true);

            subject.ViewModel.CommitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert.Invoke(subject);
        }

        public static IEnumerable CommitCommand_WhenNotImageChangedAndFailUseCase_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    1,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never))
                    )
                .SetName("CommitCommand when image not change should show alert");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Once))
                    )
                .SetName("CommitCommand when image not change should invoke usecase");

            yield return
                new TestCaseData(
                    4,
                    new Action<CommitCommandSubject>(subject =>
                        subject.NavigationService.Verify(x => x.GoBackAsync(It.IsAny<NavigationParameters>()), Times.Never))
                    )
                .SetName("CommitCommand when image not change should not go back with model");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x =>
                        x.DisplayAlertAsync(
                            "申し訳ございません！",
                            "通信エラーです。リトライしますか？。",
                            "リトライ",
                            "閉じる"), Times.Once))
                    )
                .SetName("CommitCommand when image not change and faile usecase should show alert");

            yield return
                new TestCaseData(
                    4,
                    new Action<CommitCommandSubject>(subject =>
                        Assert.True(subject.ViewModel.CommitCommand.CanExecute()))
                    )
                .SetName("CommitCommand when image not change and faile usecase and not retry should command can execute");

            yield return
                new TestCaseData(
                    4,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Once))
                    )
                .SetName("CommitCommand when image not change and faile usecase and not retry should not invoke usecase");
        }

        [Test]
        [TestCaseSource("CommitCommand_WhenNotImageChangedAndFailUseCase_TestCaseSource")]
        public void CommitCommand_WhenNotImageChangedAndFailUseCase(long advanceTime, Action<CommitCommandSubject> assert)
        {
            var subject = new CommitCommandSubject(alertResult: false);

            subject.Model.IconBase64 = null;

            subject.UpdateProfileUseCase.Setup(x => x.Invoke(It.IsAny<User>())).ReturnsObservable(new Exception());

            subject.RewardedVideoService.Setup(x => x.DisplayRewardedVideo()).ReturnsObservable(true);

            subject.ViewModel.CommitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert.Invoke(subject);
        }

        public static IEnumerable CommitCommand_WhenImageChanged_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x => x.DisplayAlertAsync(string.Empty, "動画広告を視聴して\r\nプロフィール画像を更新しよう！", "視聴する", "閉じる"), Times.Once))
                    )
                .SetName("CommitCommand when image change should show alert");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Never))
                    )
                .SetName("CommitCommand when image change and wait alert should not show reward video");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Never))
                    )
                .SetName("CommitCommand when image change and wait alert should not invoke usecase");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        Assert.False(subject.ViewModel.CommitCommand.CanExecute()))
                    )
                .SetName("CommitCommand when image change and wait alert should command can execute");

            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.NavigationService.Verify(x => x.GoBackAsync(It.IsAny<NavigationParameters>()), Times.Never))
                    )
                .SetName("CommitCommand when image change and wait alert should not go back");
        }

        [Test]
        [TestCaseSource("CommitCommand_WhenImageChanged_TestCaseSource")]
        public void CommitCommand_WhenImageChanged(long advanceTime, Action<CommitCommandSubject> assert)
        {
            var subject = new CommitCommandSubject(alertResult: null);

            subject.Model.IconBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";

            subject.UpdateProfileUseCase.Setup(x => x.Invoke(It.IsAny<User>())).ReturnsObservable();

            subject.RewardedVideoService.Setup(x => x.DisplayRewardedVideo()).ReturnsObservable(true);

            subject.ViewModel.CommitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert.Invoke(subject);
        }

        public static IEnumerable CommitCommand_WhenCancelAlert_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x => x.DisplayAlertAsync(string.Empty, "動画広告を視聴して\r\nプロフィール画像を更新しよう！", "視聴する", "閉じる"), Times.Once))
                    )
                .SetName("CommitCommand when image change should show alert");

            yield return
                new TestCaseData(
                    4,
                    new Action<CommitCommandSubject>(subject =>
                        subject.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Never))
                    )
                .SetName("CommitCommand when image change and cancel alert should not show reward video");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Never))
                    )
                .SetName("CommitCommand when image change and cancel alert should not invoke usecase");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        subject.NavigationService.Verify(x => x.GoBackAsync(It.IsAny<NavigationParameters>()), Times.Never))
                    )
                .SetName("CommitCommand when image change and cancel alert should not go back");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        Assert.True(subject.ViewModel.CommitCommand.CanExecute()))
                    )
                .SetName("CommitCommand when image change and cancel alert should command can execute");
        }

        [Test]
        [TestCaseSource("CommitCommand_WhenCancelAlert_TestCaseSource")]
        public void CommitCommand_WhenCancelAlert_ShouldComplete(long advanceTime, Action<CommitCommandSubject> assert)
        {
            var subject = new CommitCommandSubject(alertResult: false);

            subject.Model.IconBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";

            subject.UpdateProfileUseCase.Setup(x => x.Invoke(It.IsAny<User>())).ReturnsObservable();

            subject.RewardedVideoService.Setup(x => x.DisplayRewardedVideo()).ReturnsObservable(true);

            subject.ViewModel.CommitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert.Invoke(subject);
        }

        public static IEnumerable CommitCommand_WhenAcceptAlert_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x => x.DisplayAlertAsync(string.Empty, "動画広告を視聴して\r\nプロフィール画像を更新しよう！", "視聴する", "閉じる"), Times.Once))
                    )
                .SetName("CommitCommand when image change should show alert");

            yield return
                new TestCaseData(
                    4,
                    new Action<CommitCommandSubject>(subject =>
                        subject.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Once))
                    )
                .SetName("CommitCommand when image change and accept alert should show reward video");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Once))
                    )
                .SetName("CommitCommand when image change and accept alert should invoke usecase");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        Assert.True(subject.ViewModel.CommitCommand.CanExecute()))
                    )
                .SetName("CommitCommand when image change and accept alert should command can execute");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        subject.NavigationService.Verify(x => x.GoBackAsync(It.Is<NavigationParameters>(
                            parameters =>
                                parameters[ParameterNames.Model] == subject.Model
                            )), Times.Once))
                    )
                .SetName("CommitCommand when image change and accept alert should go back with model");
        }

        [Test]
        [TestCaseSource("CommitCommand_WhenAcceptAlert_TestCaseSource")]
        public void CommitCommand_WhenAcceptAlert(long advanceTime, Action<CommitCommandSubject> assert)
        {
            var subject = new CommitCommandSubject(alertResult: true);

            subject.Model.IconBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";

            subject.UpdateProfileUseCase.Setup(x => x.Invoke(It.IsAny<User>())).ReturnsObservable();

            subject.RewardedVideoService.Setup(x => x.DisplayRewardedVideo()).ReturnsObservable(true);

            subject.ViewModel.CommitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert.Invoke(subject);
        }



        public static IEnumerable CommitCommand_WhenAcceptAlertAndFailUSeCase_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    3,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x => x.DisplayAlertAsync(string.Empty, "動画広告を視聴して\r\nプロフィール画像を更新しよう！", "視聴する", "閉じる"), Times.Once))
                    )
                .SetName("CommitCommand when image change should show alert");

            yield return
                new TestCaseData(
                    4,
                    new Action<CommitCommandSubject>(subject =>
                        subject.RewardedVideoService.Verify(x => x.DisplayRewardedVideo(), Times.Once))
                    )
                .SetName("CommitCommand when image change and accept alert should show reward video");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Once))
                    )
                .SetName("CommitCommand when image change and accept alert should invoke usecase");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x =>
                        x.DisplayAlertAsync(
                            "申し訳ございません！",
                            "通信エラーです。リトライしますか？。",
                            "リトライ",
                            "閉じる"), Times.Once))
                    )
                .SetName("CommitCommand when image change and accept alert and fail usecase should show alert");

            yield return
                new TestCaseData(
                    5,
                    new Action<CommitCommandSubject>(subject =>
                        Assert.False(subject.ViewModel.CommitCommand.CanExecute()))
                    )
                .SetName("CommitCommand when image change should command not can execute");

            yield return
                new TestCaseData(
                    6,
                    new Action<CommitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x => x.DisplayAlertAsync(string.Empty, "動画広告を視聴して\r\nプロフィール画像を更新しよう！", "視聴する", "閉じる"), Times.Once))
                    )
                .SetName("CommitCommand when image change and accept alert and retry should not show alert");

            yield return
                new TestCaseData(
                    7,
                    new Action<CommitCommandSubject>(subject =>
                        subject.UpdateProfileUseCase.Verify(x => x.Invoke(subject.Model), Times.Exactly(2)))
                    )
                .SetName("CommitCommand when image change and accept alert and retry alert should invoke usecase");

            yield return
                new TestCaseData(
                    7,
                    new Action<CommitCommandSubject>(subject =>
                        subject.NavigationService.Verify(x => x.GoBackAsync(It.IsAny<NavigationParameters>()), Times.Never))
                    )
                .SetName("CommitCommand when image change and accept alert and retry should not go back");
        }

        [Test]
        [TestCaseSource("CommitCommand_WhenAcceptAlertAndFailUSeCase_TestCaseSource")]
        public void CommitCommand_WhenAcceptAlertAndFailUSeCase(long advanceTime, Action<CommitCommandSubject> assert)
        {
            var subject = new CommitCommandSubject(alertResult: true);

            subject.Model.IconBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg==";

            subject.UpdateProfileUseCase.Setup(x => x.Invoke(It.IsAny<User>())).ReturnsObservable(new Exception());

            subject.RewardedVideoService.Setup(x => x.DisplayRewardedVideo()).ReturnsObservable(true);

            subject.ViewModel.CommitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert.Invoke(subject);
        }

        #endregion

        #region ChangePhotoCommand
        [Test]
        public void ChangePhotoCommand_ShouldDisplayActionSheet()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService(true);
            var progressDialogService = Mock.ProgressDialogService<Unit>();
            var rewardedVideoService = new Mock<IRewardedVideoService>();
            var useCase = new Mock<IPickupPhotoFromAlbumUseCase>();
            var model = ModelFixture.User();
            var viewModel = new EditProfilePageViewModel(
                model: model,
                navigationService: navigationService.Object,
                pageDialogService: pageDialogService.Object)
            {
                PickupPhotoFromAlbumUseCase = useCase.Object,
                SchedulerProvider = schedulerProvider,
                ProgressDialogService = progressDialogService.Object,
                RewardedVideoService = rewardedVideoService.Object
            };

            viewModel.ChangePhotoCommand.Execute();

            pageDialogService.Verify(
                x => x.DisplayActionSheetAsync(
                    "プロフィール画像変更",
                    It.Is<IActionSheetButton[]>(
                        v =>
                            v.Length == 2
                        && v[0].IsCancel
                        && v[0].Text == "キャンセル"
                        && v[1].Text == "アルバムから選択"))
                    , Times.Once);
        }

        [Test]
        public void ChangePhotoCommand_PickupPhoto_ShouldDisplayActionSheet()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService(true);
            var progressDialogService = Mock.ProgressDialogService<Unit>();
            var rewardedVideoService = new Mock<IRewardedVideoService>();
            var useCase = new Mock<IPickupPhotoFromAlbumUseCase>();
            var model = ModelFixture.User();
            var viewModel = new EditProfilePageViewModel(
                model: model,
                navigationService: navigationService.Object,
                pageDialogService: pageDialogService.Object)
            {
                PickupPhotoFromAlbumUseCase = useCase.Object,
                SchedulerProvider = schedulerProvider,
                ProgressDialogService = progressDialogService.Object,
                RewardedVideoService = rewardedVideoService.Object
            };

            ActionSheetButton[] buttons = null;

            pageDialogService
                .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                .Returns((string name, IActionSheetButton[] v) =>
                {
                    buttons = v.Select(x => x as ActionSheetButton).ToArray();
                    return Task.CompletedTask;
                });

            viewModel.ChangePhotoCommand.Execute();

            useCase.Setup(x => x.Invoke()).ReturnsObservable(string.Empty);

            buttons[1].Action.Invoke();

            scheduler.AdvanceBy(1);

            useCase.Verify(x => x.Invoke(), Times.Once);
        }


        [Test]
        public void ChangePhotoCommand_PickupPhoto_ShouldUpdateImage()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService(true);
            var progressDialogService = Mock.ProgressDialogService<Unit>();
            var rewardedVideoService = new Mock<IRewardedVideoService>();
            var useCase = new Mock<IPickupPhotoFromAlbumUseCase>();
            var model = ModelFixture.User();
            var viewModel = new EditProfilePageViewModel(
                model: model,
                navigationService: navigationService.Object,
                pageDialogService: pageDialogService.Object)
            {
                PickupPhotoFromAlbumUseCase = useCase.Object,
                SchedulerProvider = schedulerProvider,
                ProgressDialogService = progressDialogService.Object,
                RewardedVideoService = rewardedVideoService.Object
            };

            ActionSheetButton[] buttons = null;

            pageDialogService
                .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                .Returns((string name, IActionSheetButton[] v) =>
                {
                    buttons = v.Select(x => x as ActionSheetButton).ToArray();
                    return Task.CompletedTask;
                });

            viewModel.ChangePhotoCommand.Execute();

            var expected = Faker.Lorem.Sentence();

            useCase.Setup(x => x.Invoke()).ReturnsObservable(expected);

            buttons[1].Action.Invoke();

            scheduler.AdvanceBy(1);

            Assert.That(model.IconBase64, Is.EqualTo(expected));
        }
        #endregion

        #region

        [Test]
        public void CooperationSnsCommand_ShouldDisplayActionSheet()
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService(true);
            var model = ModelFixture.User();
            var viewModel = new EditProfilePageViewModel(
                model: model,
                navigationService: navigationService.Object,
                pageDialogService: pageDialogService.Object)
            {
                SchedulerProvider = schedulerProvider
            };

            viewModel.CooperationSnsCommand.Execute();

            pageDialogService.Verify(
                x => x.DisplayActionSheetAsync(
                    "SNSでログイン",
                    It.Is<IActionSheetButton[]>(
                        v =>
                            v.Length == 4
                        && v[0].IsCancel
                        && v[0].Text == "キャンセル"
                        && v[1].Text == "Google"
                        && v[2].Text == "Twitter"
                        && v[3].Text == "LINE"))
                    , Times.Once);
        }

        [Test]
        [TestCase(1, OAuthProvider.Google, TestName = "CooperationSnsCommand When1stItemSelected ShouldInvokeOAuthSignInByGoogle")]
        [TestCase(2, OAuthProvider.Twitter, TestName = "CooperationSnsCommand When2ndItemSelected ShouldInvokeOAuthSignInByTwitter")]
        [TestCase(3, OAuthProvider.Line, TestName = "CooperationSnsCommand When3rdItemSelected ShouldInvokeOAuthSignInByLINE")]
        public void CooperationSnsCommand_WhenActionSheetSelected(int index, OAuthProvider provider)
        {
            var schedulerProvider = new SchedulerProvider();
            var scheduler = schedulerProvider.Scheduler;
            var navigationService = Mock.NavigationService();
            var pageDialogService = Mock.PageDialogService(true);
            var useCase = new Mock<IOAuthCooperationUseCase>();
            var model = ModelFixture.User();
            var viewModel = new EditProfilePageViewModel(
                model: model,
                navigationService: navigationService.Object,
                pageDialogService: pageDialogService.Object)
            {
                OAuthCooperationUseCase = useCase.Object,
                SchedulerProvider = schedulerProvider
            };

            ActionSheetButton[] buttons = null;
            pageDialogService
                .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                .Returns((string name, IActionSheetButton[] v) =>
                {
                    buttons = v.Select(x => x as ActionSheetButton).ToArray();
                    return Task.CompletedTask;
                });

            useCase.Setup(x => x.Invoke(It.IsAny<OAuthProvider>())).ReturnsObservable();

            viewModel.CooperationSnsCommand.Execute();

            buttons[index].Action.Invoke();
            useCase.Verify(x => x.Invoke(provider));
        }

        #endregion
    }
}
