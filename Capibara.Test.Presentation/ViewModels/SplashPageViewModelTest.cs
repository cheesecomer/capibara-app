#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
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
    public class SplashPageViewModelTest
    {
        [Test]
        public void LogoScaleProperty_Shoul1()
        {
            Assert.That(new SplashPageViewModel().LogoScale.Value, Is.EqualTo(1));
        }

        [Test]
        public void LogoOpacityProperty_Shoul1()
        {
            Assert.That(new SplashPageViewModel().LogoOpacity.Value, Is.EqualTo(1));
        }

        [Test]
        public void LogoTopMarginProperty_Should180()
        {
            Assert.That(new SplashPageViewModel().LogoTopMargin.Value, Is.EqualTo(180));
        }

        #region RefreshCommand

        private class RefreshCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Mock<NavigationService> NavigationService { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public Mock<IHasSessionUseCase> HasSessionUseCase { get; }

            public Mock<IRefreshSessionUseCase> RefreshSessionUseCase { get; }

            public Mock<IApplicationExitUseCase> ApplicationExitUseCase { get; }

            public SplashPageViewModel ViewModel { get; }

            public RefreshCommandSubject(bool hasSession, bool isAccepted = false, Exception exception = null, bool shouldRetry = false)
            {
                var schedulerProvider = new SchedulerProvider();

                this.Scheduler = schedulerProvider.Scheduler;
                this.NavigationService = Mock.NavigationService();
                this.PageDialogService = Mock.PageDialogService(shouldRetry);

                this.ApplicationExitUseCase = new Mock<IApplicationExitUseCase>();
                this.ApplicationExitUseCase
                    .Setup(x => x.Invoke())
                    .Returns(Task.CompletedTask);

                this.HasSessionUseCase = new Mock<IHasSessionUseCase>();
                this.HasSessionUseCase
                    .Setup(x => x.Invoke())
                    .Returns(new BehaviorSubject<bool>(hasSession));

                this.RefreshSessionUseCase = new Mock<IRefreshSessionUseCase>();
                this.RefreshSessionUseCase
                    .Setup(x => x.Invoke())
                    .Returns(() =>
                    {
                        return exception == null
                            ? Task.FromResult(ModelFixture.User(isAccepted: isAccepted))
                            : Task.FromException<User>(exception);
                    });

                this.ViewModel = new SplashPageViewModel(this.NavigationService.Object, this.PageDialogService.Object)
                {
                    SchedulerProvider = schedulerProvider,
                    ApplicationExitUseCase = this.ApplicationExitUseCase.Object,
                    HasSessionUseCase = this.HasSessionUseCase.Object,
                    RefreshSessionUseCase = this.RefreshSessionUseCase.Object
                };
            }

            public static RefreshCommandSubject WhenHasNotSession() => new RefreshCommandSubject(false);

            public static RefreshCommandSubject WhenHasSession() => new RefreshCommandSubject(true, true);

            public static RefreshCommandSubject WhenHasSessionAndNeedAccept() => new RefreshCommandSubject(true, false);

            public static RefreshCommandSubject WhenHasSessionAndUnauthorized() => new RefreshCommandSubject(true, exception: new UnauthorizedException());

            public static RefreshCommandSubject WhenHasSessionAndFailedAndNotRetry() => new RefreshCommandSubject(true, exception: new Exception(), shouldRetry: false);

            public static RefreshCommandSubject WhenHasSessionAndFailedAndRetry() => new RefreshCommandSubject(true, exception: new Exception(), shouldRetry: true);
        }

        #region WhenHasNotSession

        [TestCase(0, 180d, TestName = "RefreshCommand WhenHasNotSession {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(11, 180d - (180d - 20d) / (500d / 10d) * 1, TestName = "RefreshCommand WhenHasNotSession {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(101, 180d - (180d - 20d) / (500d / 10d) * 10, TestName = "RefreshCommand WhenHasNotSession {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(111, 180d - (180d - 20d) / (500d / 10d) * 11, TestName = "RefreshCommand WhenHasNotSession {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(491, 180d - (180d - 20d) / (500d / 10d) * 49, TestName = "RefreshCommand WhenHasNotSession {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(501, 180d - (180d - 20d) / (500d / 10d) * 50, TestName = "RefreshCommand WhenHasNotSession {0}ms after Should LogoTopMargin change to expected")]
        public void RefreshCommand_WhenHasNotSession_ShouldAnimatedLogoTopMargin(int milliseconds, double expected)
        {
            var subject = RefreshCommandSubject.WhenHasNotSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(milliseconds).Ticks);

            Assert.That(subject.ViewModel.LogoTopMargin.Value, Is.EqualTo(expected));
        }

        [Test]
        public void RefreshCommand_WhenHasNotSessiont_ShouldNotAnimatedLogoScale()
        {
            var subject = RefreshCommandSubject.WhenHasNotSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);

            Assert.That(subject.ViewModel.LogoScale.Value, Is.EqualTo(1));
        }

        [Test]
        public void RefreshCommand_WhenHasNotSessiont_ShouldNotAnimatedLogoOpacity()
        {
            var subject = RefreshCommandSubject.WhenHasNotSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);

            Assert.That(subject.ViewModel.LogoOpacity.Value, Is.EqualTo(1));
        }

        [Test]
        public void RefreshCommand_WhenHasNotSession_ShouldNavigateToSignUpPage()
        {
            var subject = RefreshCommandSubject.WhenHasNotSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(511).Ticks);

            subject.NavigationService.Verify(x => x.NavigateAsync("SignUpPage", null, null, false), Times.Once());
        }

        #endregion

        #region WhenHasSession

        [Test]
        public void RefreshCommand_WhenHasSession_ShouldInvokeRefreshSessionUseCase()
        {
            var subject = RefreshCommandSubject.WhenHasSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        [Test]
        public void RefreshCommand_WhenHasSession_ShouldNotAnimatedLogoTopMargin()
        {
            var subject = RefreshCommandSubject.WhenHasSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);

            Assert.That(subject.ViewModel.LogoTopMargin.Value, Is.EqualTo(180d));
        }

        [TestCase(0, 1, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoScale change to expected")]
        [TestCase(11, 1 + 2d / (500d / 10d) * 1, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoScale change to expected")]
        [TestCase(101, 1 + 2d / (500d / 10d) * 10, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoScale change to expected")]
        [TestCase(111, 1 + 2d / (500d / 10d) * 11, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoScale change to expected")]
        [TestCase(491, 1 + 2d / (500d / 10d) * 49, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoScale change to expected")]
        [TestCase(501, 3, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoScale change to expected")]
        public void RefreshCommand_WhenHasSession_ShouldAnimatedLogoScale(int milliseconds, double expected)
        {
            var subject = RefreshCommandSubject.WhenHasSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(milliseconds).Ticks);

            Assert.That(subject.ViewModel.LogoScale.Value, Is.EqualTo(expected));
        }

        [TestCase(0, 1, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoOpacity change to expected")]
        [TestCase(11, 1 - 1d / (500d / 10d) * 1, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoOpacity change to expected")]
        [TestCase(101, 1 - 1d / (500d / 10d) * 10, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoOpacity change to expected")]
        [TestCase(111, 1 - 1d / (500d / 10d) * 11, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoOpacity change to expected")]
        [TestCase(491, 1 - 1d / (500d / 10d) * 49, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoOpacity change to expected")]
        [TestCase(501, 0, TestName = "RefreshCommand WhenHasSession {0}ms after Should LogoOpacity change to expected")]
        public void RefreshCommand_WhenHasSession_ShouldAnimatedLogoOpacity(int milliseconds, double expected)
        {
            var subject = RefreshCommandSubject.WhenHasSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(milliseconds).Ticks);

            Assert.That(subject.ViewModel.LogoOpacity.Value, Is.EqualTo(expected));
        }

        [Test]
        public void RefreshCommand_WhenHasSession_ShouldNavigateToMainPage()
        {
            var subject = RefreshCommandSubject.WhenHasSession();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(511).Ticks);

            subject.NavigationService.Verify(x => x.NavigateAsync("/NavigationPage/MainPage", null, null, false), Times.Once());
        }

        #endregion

        #region WhenHasSessionAndNeedAccept

        [Test]
        public void RefreshCommand_WhenHasSessionAndNeedAccept_ShouldInvokeRefreshSessionUseCase()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndNeedAccept();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(1);

            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndNeedAccept_ShouldNotAnimatedLogoTopMargin()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndNeedAccept();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);

            Assert.That(subject.ViewModel.LogoTopMargin.Value, Is.EqualTo(180d));
        }

        [TestCase(0, 1, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoScale change to expected")]
        [TestCase(11, 1 + 2d / (500d / 10d) * 1, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoScale change to expected")]
        [TestCase(101, 1 + 2d / (500d / 10d) * 10, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoScale change to expected")]
        [TestCase(111, 1 + 2d / (500d / 10d) * 11, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoScale change to expected")]
        [TestCase(491, 1 + 2d / (500d / 10d) * 49, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoScale change to expected")]
        [TestCase(501, 3, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoScale change to expected")]
        public void RefreshCommand_WhenHasSessionAndNeedAccept_ShouldAnimatedLogoScale(int milliseconds, double expected)
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndNeedAccept();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(milliseconds).Ticks);

            Assert.That(subject.ViewModel.LogoScale.Value, Is.EqualTo(expected));
        }

        [TestCase(0, 1, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoOpacity change to expected")]
        [TestCase(11, 1 - 1d / (500d / 10d) * 1, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoOpacity change to expected")]
        [TestCase(101, 1 - 1d / (500d / 10d) * 10, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoOpacity change to expected")]
        [TestCase(111, 1 - 1d / (500d / 10d) * 11, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoOpacity change to expected")]
        [TestCase(491, 1 - 1d / (500d / 10d) * 49, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoOpacity change to expected")]
        [TestCase(501, 0, TestName = "RefreshCommand WhenHasSessionAndNeedAccept {0}ms after Should LogoOpacity change to expected")]
        public void RefreshCommand_WhenHasSessionAndNeedAccept_ShouldAnimatedLogoOpacity(int milliseconds, double expected)
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndNeedAccept();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(milliseconds).Ticks);

            Assert.That(subject.ViewModel.LogoOpacity.Value, Is.EqualTo(expected));
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndNeedAccept_ShouldNavigateToMainPage()
        {
            var subject = new RefreshCommandSubject(true, false);

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(511).Ticks);

            subject.NavigationService.Verify(x => x.NavigateAsync("/NavigationPage/AcceptPage", It.IsAny<INavigationParameters>(), null, false), Times.Once());
        }

        #endregion

        #region WhenHasSessionAndUnauthorized

        [TestCase(0, 180d, TestName = "RefreshCommand WhenHasSessionAndUnauthorized {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(11, 180d - (180d - 20d) / (500d / 10d) * 1, TestName = "RefreshCommand WhenHasSessionAndUnauthorized {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(101, 180d - (180d - 20d) / (500d / 10d) * 10, TestName = "RefreshCommand WhenHasSessionAndUnauthorized {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(111, 180d - (180d - 20d) / (500d / 10d) * 11, TestName = "RefreshCommand WhenHasSessionAndUnauthorized {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(491, 180d - (180d - 20d) / (500d / 10d) * 49, TestName = "RefreshCommand WhenHasSessionAndUnauthorized {0}ms after Should LogoTopMargin change to expected")]
        [TestCase(501, 180d - (180d - 20d) / (500d / 10d) * 50, TestName = "RefreshCommand WhenHasSessionAndUnauthorized {0}ms after Should LogoTopMargin change to expected")]
        public void RefreshCommand_WhenHasSessionAndUnauthorized_ShouldAnimatedLogoTopMargin(int milliseconds, double expected)
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndUnauthorized();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(milliseconds).Ticks);

            Assert.That(subject.ViewModel.LogoTopMargin.Value, Is.EqualTo(expected));
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndUnauthorized_ShouldNotAnimatedLogoScale()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndUnauthorized();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);

            Assert.That(subject.ViewModel.LogoScale.Value, Is.EqualTo(1));
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndUnauthorized_ShouldNotAnimatedLogoOpacity()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndUnauthorized();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(501).Ticks);

            Assert.That(subject.ViewModel.LogoOpacity.Value, Is.EqualTo(1));
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndUnauthorized_ShouldNavigateToSignUpPage()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndUnauthorized();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(TimeSpan.FromMilliseconds(511).Ticks);

            subject.NavigationService.Verify(x => x.NavigateAsync("SignUpPage", null, null, false), Times.Once());
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndUnauthorized_ShouldNotDisplayAlert()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndUnauthorized();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1);
            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);
            subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        #endregion

        #region WhenHasSessionAndFailedAndNotRetry

        [Test]
        public void RefreshCommand_WhenHasSessionAndFailedAndNotRetry_ShouldDisplayAlert()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndFailedAndNotRetry();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1);
            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);
            subject.Scheduler.AdvanceBy(1);
            subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void RefreshCommand_WhenHasSessionAndFailedAndNotRetry_ShouldExit()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndFailedAndNotRetry();

            subject.ViewModel.RefreshCommand.Execute();

            subject.Scheduler.AdvanceBy(1);
            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);
            subject.Scheduler.AdvanceBy(1);
            subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            subject.Scheduler.AdvanceBy(1);
            subject.ApplicationExitUseCase.Verify(x => x.Invoke(), Times.Once);
        }

        #endregion

        #region WhenHasSessionAndFailedNotRetry

        [Test]
        public void RefreshCommand_WhenHasSessionAndFailedAndRetry_ShouldInvokeRefreshSessionUseCaseTwice()
        {
            var subject = RefreshCommandSubject.WhenHasSessionAndFailedAndRetry();

            subject.ViewModel.RefreshCommand.Execute();
            subject.Scheduler.AdvanceBy(1);
            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Once);

            subject.Scheduler.AdvanceBy(1);
            subject.Scheduler.AdvanceBy(1);
            subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);

            subject.Scheduler.AdvanceBy(1);
            subject.RefreshSessionUseCase.Verify(x => x.Invoke(), Times.Exactly(2));
        }

        #endregion

        #endregion
    }
}
