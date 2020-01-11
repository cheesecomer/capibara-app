#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System;
using System.Collections;

using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Moq;
using NUnit.Framework;
using Prism.Navigation;
using Prism.Services;
using Microsoft.Reactive.Testing;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class InquiryPageViewModelTest
    {
        #region SubmitCommand

        public static IEnumerable SubmitCommandCanExecute_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    string.Empty,
                    string.Empty,
                    false)
                .SetName("SubmitCommand.CanExecute when Email and Message is empty should can not execute");

            yield return
                new TestCaseData(
                    null,
                    null,
                    false)
                .SetName("SubmitCommand.CanExecute when Email and Message is null should can not execute");

            yield return
                new TestCaseData(
                    string.Empty,
                    Faker.Lorem.Paragraph(),
                    false)
                .SetName("SubmitCommand.CanExecute when Email is empty should can not execute");

            yield return
                new TestCaseData(
                    null,
                    Faker.Lorem.Paragraph(),
                    false)
                .SetName("SubmitCommand.CanExecute when Email is null should can not execute");

            yield return
                new TestCaseData(
                    Faker.Internet.Email(),
                    string.Empty,
                    false)
                .SetName("SubmitCommand.CanExecute when Message is empty should can not execute");

            yield return
                new TestCaseData(
                    Faker.Internet.Email(),
                    null,
                    false)
                .SetName("SubmitCommand.CanExecute when Message is null should can not execute");

            yield return
                new TestCaseData(
                    Faker.Internet.Email(),
                    Faker.Lorem.Paragraph(),
                    true)
                .SetName("SubmitCommand.CanExecute when Email and Message is present should can execute");
        }

        [Test]
        [TestCaseSource("SubmitCommandCanExecute_TestCaseSource")]
        public void SubmitCommandCanExecute(string email, string message, bool canExecute)
        {
            var viewModel = new InquiryPageViewModel();

            viewModel.Model.Email = email;
            viewModel.Model.Message = message;

            Assert.That(viewModel.SubmitCommand.CanExecute(), Is.EqualTo(canExecute));
        }

        public class SubmitCommandSubject
        {
            public TestScheduler Scheduler { get; }

            public Mock<ICreateInquiryUseCase> CreateInquiryUseCase { get; }

            public Mock<NavigationService> NavigationService { get; }

            public Mock<IPageDialogService> PageDialogService { get; }

            public InquiryPageViewModel ViewModel { get; }

            public Inquiry Model => this.ViewModel.Model;

            public SubmitCommandSubject()
            {
                var schedulerProvider = new SchedulerProvider();
                var useCase = new Mock<ICreateInquiryUseCase>();
                var navigationService = Mock.NavigationService();
                var pageDialogService = Mock.PageDialogService(null);
                var viewModel = new InquiryPageViewModel(
                    navigationService.Object,
                    pageDialogService.Object)
                {
                    SchedulerProvider = schedulerProvider,
                    CreateInquiryUseCase = useCase.Object,
                };

                this.ViewModel = viewModel;
                this.NavigationService = navigationService;
                this.PageDialogService = pageDialogService;
                this.Scheduler = schedulerProvider.Scheduler;
                this.CreateInquiryUseCase = useCase;
            }
        }

        public static IEnumerable SubmitCommand_WhenSuccess_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    2,
                    new Action<SubmitCommandSubject>(subject => subject.CreateInquiryUseCase.Verify(x => x.Invoke(subject.Model), Times.Once)))
                .SetName("SubmitCommand when execute should invoke usecase");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => subject.NavigationService.Verify(x => x.GoBackAsync(), Times.Once)))
                .SetName("SubmitCommand when usecase success should go back");
        }

        [Test]
        [TestCaseSource("SubmitCommand_WhenSuccess_TestCaseSource")]
        public void SubmitCommand_WhenSuccess(int advanceTime, Action<SubmitCommandSubject> assert)
        {
            var subject = new SubmitCommandSubject();

            subject.ViewModel.Email.Value = Faker.Internet.Email();
            subject.ViewModel.Message.Value = Faker.Lorem.Paragraph();

            subject.CreateInquiryUseCase.Setup(x => x.Invoke(It.IsAny<Inquiry>())).ReturnsObservable();

            subject.ViewModel.SubmitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        public static IEnumerable SubmitCommand_WhenFail_TestCaseSource()
        {
            yield return
                new TestCaseData(
                    2,
                    new Action<SubmitCommandSubject>(subject => subject.CreateInquiryUseCase.Verify(x => x.Invoke(subject.Model), Times.Once)))
                .SetName("SubmitCommand when execute should invoke usecase");

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
                .SetName("SubmitCommand when usecase fail should show retry dialog");

            yield return
                new TestCaseData(
                    3,
                    new Action<SubmitCommandSubject>(subject => subject.NavigationService.Verify(x => x.GoBackAsync(), Times.Never)))
                .SetName("SubmitCommand when usecase fail should not go back");
        }

        [Test]
        [TestCaseSource("SubmitCommand_WhenFail_TestCaseSource")]
        public void SubmitCommand_WhenFail(int advanceTime, Action<SubmitCommandSubject> assert)
        {
            var subject = new SubmitCommandSubject();

            subject.ViewModel.Email.Value = Faker.Internet.Email();
            subject.ViewModel.Message.Value = Faker.Lorem.Paragraph();

            subject.CreateInquiryUseCase.Setup(x => x.Invoke(It.IsAny<Inquiry>())).ReturnsObservable(new Exception());

            subject.ViewModel.SubmitCommand.Execute();

            subject.Scheduler.AdvanceBy(advanceTime);

            assert(subject);
        }

        #endregion
    }
}
