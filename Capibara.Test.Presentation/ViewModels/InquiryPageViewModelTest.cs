#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using System.Collections;

using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Moq;
using NUnit.Framework;
using Prism.Navigation;

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

        [Test]
        public void SubmitCommand_WhenSuccess()
        {
            var schedulerProvider = new SchedulerProvider();
            var useCase = new Mock<ICreateInquiryUseCase>();
            var navigationService = Mock.NavigationService();
            var viewModel = new InquiryPageViewModel(navigationService.Object)
            {
                SchedulerProvider = schedulerProvider,
                CreateInquiryUseCase = useCase.Object,
            };

            viewModel.Email.Value = Faker.Internet.Email();
            viewModel.Message.Value = Faker.Lorem.Paragraph();

            useCase.Setup(x => x.Invoke(It.IsAny<Inquiry>())).ReturnsObservable();

            viewModel.SubmitCommand.Execute();

            schedulerProvider.Scheduler.AdvanceBy(1);
            schedulerProvider.Scheduler.AdvanceBy(1);

            useCase.Verify(x => x.Invoke(viewModel.Model), Times.Once);

            schedulerProvider.Scheduler.AdvanceBy(1);

            navigationService.Verify(x => x.GoBackAsync(), Times.Once);
        }

        #endregion
    }
}
