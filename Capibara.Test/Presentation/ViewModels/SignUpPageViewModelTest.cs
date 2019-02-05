#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Prism.Navigation;
using Prism.Services;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class SignUpPageViewModelTest
    {
        [Test]
        public void NicknameProperty_WhenDefault_ShouldEmpty()
        {
            Assert.That(new SignUpPageViewModel().Nickname.Value, Is.EqualTo(string.Empty));
        }

        #region SignUpCommand

        [Test]
        [TestCase(false, TestName = "SignUpCommand WhenNicknameIsEmpty ShouldCanNotExecute")]
        [TestCase(true, TestName = "SignUpCommand WhenNicknameIsPresece ShouldCanExecute")]
        public void SignUpCommand_CanNotExecute(bool isPresece)
        {
            var subject = new SignUpPageViewModel();
            subject.Nickname.Value = isPresece ? Faker.Name.FullName() : string.Empty;

            Assert.That(subject.SignUpCommand.CanExecute(), Is.EqualTo(isPresece));
        }

        class SignUpCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Mock<NavigationService> NavigationService { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public Mock<ISignUpUseCase> SignUpUseCase { get; }

            public Mock<IOAuthSignIn> OAuthSignIn { get; }

            public SignUpPageViewModel ViewModel { get; }

            public SignUpCommandSubject(Exception exception = null, bool isAccepted = false)
            {
                var schedulerProvider = new SchedulerProvider();
                
                this.Scheduler = schedulerProvider.Scheduler;
                this.NavigationService = Mock.NavigationService();
                this.PageDialogService = Mock.PageDialogService();

                this.SignUpUseCase = new Mock<ISignUpUseCase>();
                this.SignUpUseCase
                    .Setup(x => x.Invoke(It.IsAny<string>()))
                    .Returns(exception == null
                        ? Task.FromResult(ModelFixture.User(isAccepted: isAccepted))
                        : Task.FromException<User>(exception));

                this.OAuthSignIn = new Mock<IOAuthSignIn>();

                this.ViewModel = new SignUpPageViewModel(this.NavigationService.Object, this.PageDialogService.Object)
                {
                    SchedulerProvider = schedulerProvider,
                    SignUpUseCase = this.SignUpUseCase.Object,
                    OAuthSignIn = this.OAuthSignIn.Object
                };
            }

            public static SignUpCommandSubject WhenSignUpFailed() => new SignUpCommandSubject(new Exception());

            public static SignUpCommandSubject WhenSignUpSuccess(bool isAccepted) => new SignUpCommandSubject(isAccepted: isAccepted);
        }

        [Test]
        public void SignUpCommand_ShouldInvokeSingUp()
        {
            var subject = new SignUpCommandSubject();

            subject.ViewModel.Nickname.Value = Faker.Name.FullName();
            subject.ViewModel.SignUpCommand.Execute();

            subject.SignUpUseCase.Verify(x => x.Invoke(subject.ViewModel.Nickname.Value), Times.Once);
        }

        [Test]
        public void SignUpCommand_WhenSignUpCommandRunning_ShouldCanNotExecute()
        {
            var subject = SignUpCommandSubject.WhenSignUpFailed();

            subject.ViewModel.Nickname.Value = Faker.Name.FullName();
            subject.ViewModel.SignUpCommand.Execute();

            Assert.That(subject.ViewModel.SignUpCommand.CanExecute(), Is.EqualTo(false));
        }

        [Test]
        public void SignUpCommand_WhenSignUpFailed_ShouldDisplayAlert()
        {
            var subject = SignUpCommandSubject.WhenSignUpFailed();

            subject.ViewModel.Nickname.Value = Faker.Name.FullName();
            subject.ViewModel.SignUpCommand.Execute();

            subject.Scheduler.AdvanceBy(1);
            subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region SignUpWithSnsCommand

        class SignUpWithSnsCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Mock<NavigationService> NavigationService { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public Mock<ISignUpUseCase> SignUpUseCase { get; }

            public SignUpPageViewModel ViewModel { get; }

            public SignUpWithSnsCommandSubject(Exception exception = null, bool isAccepted = false)
            {
                var schedulerProvider = new SchedulerProvider();

                this.Scheduler = schedulerProvider.Scheduler;
                this.NavigationService = Mock.NavigationService();
                this.PageDialogService = Mock.PageDialogService();

                this.SignUpUseCase = new Mock<ISignUpUseCase>();
                this.SignUpUseCase
                    .Setup(x => x.Invoke(It.IsAny<string>()))
                    .Returns(exception == null
                        ? Task.FromResult(ModelFixture.User(isAccepted: isAccepted))
                        : Task.FromException<User>(exception));

                this.ViewModel = new SignUpPageViewModel(this.NavigationService.Object, this.PageDialogService.Object)
                {
                    SchedulerProvider = schedulerProvider,
                    SignUpUseCase = this.SignUpUseCase.Object
                };
            }
        }

        [Test]
        public void SignUpWithSnsCommand_WhenSignUpCommandRunning_ShouldCanNotExecute()
        {
            var subject = SignUpCommandSubject.WhenSignUpFailed();

            subject.ViewModel.Nickname.Value = Faker.Name.FullName();
            subject.ViewModel.SignUpCommand.Execute();

            Assert.That(subject.ViewModel.SignUpWithSnsCommand.CanExecute(), Is.EqualTo(false));
        }

        [Test]
        public void SignUpWithSnsCommand_ShouldDisplayActionSheet()
        {
            var subject = SignUpCommandSubject.WhenSignUpFailed();

            subject.ViewModel.Nickname.Value = Faker.Name.FullName();
            subject.ViewModel.SignUpWithSnsCommand.Execute();

            subject.PageDialogService.Verify(
                x => x.DisplayActionSheetAsync(
                    "SNSでログイン",
                    It.Is<IActionSheetButton[]>(
                        v =>
                            v.Length == 4
                        &&  v[0].IsCancel
                        &&  v[0].Text == "キャンセル"
                        &&  v[1].Text == "Google"
                        &&  v[2].Text == "Twitter"
                        &&  v[3].Text == "LINE"))
                    , Times.Once);
        }

        [Test]
        [TestCase(1, OAuthProvider.Google, TestName = "SignUpWithSnsCommand When1stItemSelected ShouldInvokeOAuthSignInByGoogle")]
        [TestCase(2, OAuthProvider.Twitter, TestName = "SignUpWithSnsCommand When2ndItemSelected ShouldInvokeOAuthSignInByTwitter")]
        [TestCase(3, OAuthProvider.Line, TestName = "SignUpWithSnsCommand When3rdItemSelected ShouldInvokeOAuthSignInByLINE")]
        public void SignUpWithSnsCommand_WhenActionSheetSelected(int index, OAuthProvider provider)
        {
            var subject = SignUpCommandSubject.WhenSignUpFailed();

            ActionSheetButton[] buttons = null;
            subject.PageDialogService
                .Setup(x => x.DisplayActionSheetAsync(It.IsAny<string>(), It.IsAny<IActionSheetButton[]>()))
                .Returns((string name, IActionSheetButton[] v) =>
                {
                    buttons = v.Select(x => x as ActionSheetButton).ToArray();
                    return Task.CompletedTask;
                });

            subject.ViewModel.Nickname.Value = Faker.Name.FullName();
            subject.ViewModel.SignUpWithSnsCommand.Execute();

            buttons[index].Action.Invoke();
            subject.OAuthSignIn.Verify(x => x.Invoke(provider));
        }

        #endregion

        #region RefreshCommand

        private class RefreshCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public BehaviorSubject<bool> HasSessionSubject { get; }

            public Mock<NavigationService> NavigationService { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public Mock<IHasSessionUseCase> HasSessionUseCase { get; }

            public Mock<IRefreshSessionUseCase> RefreshSessionUseCase { get; }

            public SignUpPageViewModel ViewModel { get; }

            public RefreshCommandSubject(bool isAccepted = false, Exception exception = null, bool shouldRetry = false)
            {
                var schedulerProvider = new SchedulerProvider();

                this.Scheduler = schedulerProvider.Scheduler;
                this.NavigationService = Mock.NavigationService();
                this.PageDialogService = Mock.PageDialogService();

                var retryStack = new Stack<bool>();
                retryStack.Push(shouldRetry);
                this.PageDialogService
                    .Setup(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(() =>
                    {
                        return Task.FromResult(retryStack.Peek());
                    });

                this.HasSessionSubject = new BehaviorSubject<bool>(false);

                this.HasSessionUseCase = new Mock<IHasSessionUseCase>();
                this.HasSessionUseCase
                    .Setup(x => x.Invoke())
                    .Returns(this.HasSessionSubject);

                this.RefreshSessionUseCase = new Mock<IRefreshSessionUseCase>();
                this.RefreshSessionUseCase
                    .Setup(x => x.Invoke())
                    .Returns(() =>
                    {
                        return exception == null
                            ? Task.FromResult(ModelFixture.User(isAccepted: isAccepted))
                            : Task.FromException<User>(exception);
                    });

                this.ViewModel = new SignUpPageViewModel(this.NavigationService.Object, this.PageDialogService.Object)
                {
                    SchedulerProvider = schedulerProvider,
                    HasSessionUseCase = this.HasSessionUseCase.Object,
                    RefreshSessionUseCase = this.RefreshSessionUseCase.Object
                };
            }

            public static RefreshCommandSubject WhenHasNotSession() => new RefreshCommandSubject();

            public static RefreshCommandSubject WhenHasSessionAndAccepted() => new RefreshCommandSubject(true);

            public static RefreshCommandSubject WhenHasSessionAndNotAccepted() => new RefreshCommandSubject(false);

            public static RefreshCommandSubject WhenHasSessionAndFailedAndNotRetry() => new RefreshCommandSubject(exception: new Exception(), shouldRetry: false);

            public static RefreshCommandSubject WhenHasSessionAndFailedAndRetry() => new RefreshCommandSubject(exception: new Exception(), shouldRetry: true);
        }

        [Test]
        public void RefreshCommand_ShouldInvokeHasSessionUseCase()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndAccepted();

            subject.ViewModel.RefreshCommand.Execute();
            subject.HasSessionUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        #region WhenHasNotSession

        [Test]
        public void RefreshCommand_WhenHasNotSession_ShouldNotInvokeRefreshSessionUseCase()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndAccepted();

            subject.ViewModel.RefreshCommand.Execute();
            subject.HasSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.HasSessionSubject.OnNext(false);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Never);
        }

        #endregion

        #region WhenHasSessionAndAccepted

        [Test]
        public void RefreshCommand_WhenHasSessionAndAccepted_ShouldInvokeRefreshSessionUseCase()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndAccepted();

            subject.ViewModel.RefreshCommand.Execute();
            subject.HasSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);

            subject.HasSessionSubject.OnNext(true);

            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndAccepted_ShouldNavigateToMainPage()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndAccepted();

            subject.ViewModel.RefreshCommand.Execute();
            subject.HasSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);

            subject.HasSessionSubject.OnNext(true);

            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);

            subject.NavigationService.Verify(x => x.NavigateAsync("/NavigationPage/MainPage", null, null, false), Times.Once());
        }

        #endregion

        #region WhenHasSessionAndNotAccepted

        [Test]
        public void RefreshCommand_WhenHasSessionAndNotAccepted_ShouldNavigateToMainPage()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndNotAccepted();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1);

            subject.HasSessionSubject.OnNext(true);

            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);

            subject.NavigationService.Verify(x => x.NavigateAsync("/NavigationPage/AcceptPage", It.IsNotNull<INavigationParameters>(), null, false), Times.Once());
        }

        #endregion

        #region WhenHasSessionAndFailedAndNotRetry

        [Test]
        public void RefreshCommand_WhenHasSessionAndFailedAndNotRetry_ShouldDisplayAlert()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndFailedAndNotRetry();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1);

            subject.HasSessionSubject.OnNext(true);

            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);
            subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        #endregion

        #region WhenHasSessionAndFailedNotRetry

        [Test]
        public void RefreshCommand_WhenHasSessionAndFailedAndRetry_ShouldInvokeRefreshSessionUseCaseTwice()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndFailedAndRetry();
            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1);

            subject.HasSessionSubject.OnNext(true);

            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);

            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Exactly(2));
        }

        #endregion

        #endregion

    }
}
