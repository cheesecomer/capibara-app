#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;
using Prism.Services;

namespace Capibara.Presentation.ViewModels
{
    public class ReportPageViewModelTest
    {
        [Test]
        public void ReportReasonsTest()
        {
            var viewModel = new ReportPageViewModel();
            var expected = new[]
            {
                ReportReason.Spam,
                ReportReason.AbusiveOrHatefulSpeech,
                ReportReason.AbusiveOrHatefulImage,
                ReportReason.ObsceneSpeech,
                ReportReason.ObsceneImage,
                ReportReason.Other
            };

            CollectionAssert.AreEqual(expected, viewModel.ReportReasons);
        }

        public static IEnumerable SubmitCommandCanExecute_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    null,
                    string.Empty,
                    false)
                .SetName("SubmitCommand.CanExecute when Reason is null and Message is empty should can not execute");

            yield return
                new TestCaseData(
                    null,
                    null,
                    false)
                .SetName("SubmitCommand.CanExecute when Reason and Message is null should can not execute");

            yield return
                new TestCaseData(
                    null,
                    Faker.Lorem.Paragraph(),
                    false)
                .SetName("SubmitCommand.CanExecute when Reason is null should can not execute");

            yield return
                new TestCaseData(
                    ReportReason.Spam,
                    string.Empty,
                    false)
                .SetName("SubmitCommand.CanExecute when  Message is empty should can not execute");

            yield return
                new TestCaseData(
                    ReportReason.Spam,
                    null,
                    false)
                .SetName("SubmitCommand.CanExecute when  Message is null should can not execute");

            yield return
                new TestCaseData(
                    ReportReason.Spam,
                    Faker.Lorem.Paragraph(),
                    true)
                .SetName("SubmitCommand.CanExecute when Reason and Message is present should can execute");
        }

        [Test]
        [TestCaseSource("SubmitCommandCanExecute_TestCaseSource")]
        public void SubmitCommandCanExecute(ReportReason? reason, string message, bool canExecute)
        {
            var viewModel = new ReportPageViewModel();

            viewModel.Reason.Value = reason;
            viewModel.Message.Value = message;

            Assert.That(viewModel.SubmitCommand.CanExecute(), Is.EqualTo(canExecute));
        }

        public class SubmitCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Mock<ICreateReportUseCase> CreateReportUseCase { get; }

            public Mock<NavigationService> NavigationService { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public ReportPageViewModel ViewModel { get; }

            public Report Model => this.ViewModel.Model;

            public SubmitCommandSubject()
            {
                var schedulerProvider = new SchedulerProvider();
                var useCase = new Mock<ICreateReportUseCase>();
                var navigationService = Mock.NavigationService();
                var pageDialogService = Mock.PageDialogService(true);

                var viewModel = new ReportPageViewModel(
                    navigationService.Object,
                    pageDialogService.Object)
                {
                    SchedulerProvider = schedulerProvider,
                    CreateReportUseCase = useCase.Object,
                };

                this.ViewModel = viewModel;
                this.NavigationService = navigationService;
                this.PageDialogService = pageDialogService;
                this.Scheduler = schedulerProvider.Scheduler;
                this.CreateReportUseCase = useCase;

                viewModel.Reason.Value = ReportReason.Spam;
                viewModel.Message.Value = Faker.Lorem.Paragraph();
            }
        }

        public static IEnumerable SubmitCommandExecute_WhenSuccess_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    2,
                    new Action<SubmitCommandSubject>(subject => subject.CreateReportUseCase.Verify(x => x.Invoke(subject.Model), Times.Once)))
                .SetName("SubmitCommand.Execute should invoke usecase");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => subject.NavigationService.Verify(x => x.GoBackAsync(), Times.Once)))
                .SetName("SubmitCommand.Execute when success should go back");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => Assert.True(subject.ViewModel.SubmitCommand.CanExecute())))
                .SetName("SubmitCommand.Execute when success should complete");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never)))
                .SetName("SubmitCommand.Execute when success should not show dialog");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never)))
                .SetName("SubmitCommand.Execute when success should not show dialog");
        }

        [Test]
        [TestCaseSource("SubmitCommandExecute_WhenSuccess_TestCaseSource")]
        public void SubmitCommandExecute_WhenSuccess(int advanceTime, Action<SubmitCommandSubject> assert)
        {
            var subject = new SubmitCommandSubject();

            subject.CreateReportUseCase.Setup(x => x.Invoke(It.IsAny<Report>())).ReturnsObservable();

            subject.ViewModel.SubmitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        public static IEnumerable SubmitCommandExecute_WhenFail_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    2,
                    new Action<SubmitCommandSubject>(subject => subject.CreateReportUseCase.Verify(x => x.Invoke(subject.Model), Times.Once)))
                .SetName("SubmitCommand.Execute should invoke usecase");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => subject.NavigationService.Verify(x => x.GoBackAsync(), Times.Never)))
                .SetName("SubmitCommand.Execute when fail should not go back");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => Assert.False(subject.ViewModel.SubmitCommand.CanExecute())))
                .SetName("SubmitCommand.Execute when fail should not complete");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject =>
                        subject.PageDialogService.Verify(x =>
                            x.DisplayAlertAsync(
                                "申し訳ございません！",
                                "通信エラーです。リトライしますか？。",
                                "リトライ",
                                "閉じる"), Times.Once)))
                .SetName("SubmitCommand.Execute when fail should show error dialog");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => subject.PageDialogService.Verify(x => x.DisplayAlertAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never)))
                .SetName("SubmitCommand.Execute when fail should not show dialog");

            yield return
                new TestCaseData(
                    5,
                    new Action<SubmitCommandSubject>(subject => subject.CreateReportUseCase.Verify(x => x.Invoke(subject.Model), Times.Exactly(2))))
                .SetName("SubmitCommand.Execute when fail should usecase retry");
        }

        [Test]
        [TestCaseSource("SubmitCommandExecute_WhenFail_TestCaseSource")]
        public void SubmitCommandExecute_WhenFail(int advanceTime, Action<SubmitCommandSubject> assert)
        {
            var subject = new SubmitCommandSubject();

            subject.CreateReportUseCase.Setup(x => x.Invoke(It.IsAny<Report>())).ReturnsObservable(new Exception());

            subject.ViewModel.SubmitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }
    }
}
