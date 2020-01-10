#pragma warning disable CS1701 // アセンブリ参照が ID と一致すると仮定します
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Moq;
using NUnit.Framework;
using Prism.Navigation;

namespace Capibara.Presentation.ViewModels
{
    [TestFixture]
    public class InboxPageViewModelTest
    {
        #region RefreshCommand

        [Test]
        public void RefreshCommand_WhenSuccess()
        {
            var schedulerProvider = new SchedulerProvider();
            var messages = ModelFixture.MessageCollection();
            var useCase = new Mock<IFetchDirectMessageThreadUseCase>();
            var viewModel = new InboxPageViewModel
            {
                FetchDirectMessageThreadUseCase = useCase.Object,
                SchedulerProvider = schedulerProvider
            };

            useCase.Setup(x => x.Invoke()).ReturnsObservable(messages);

            viewModel.RefreshCommand.Execute();

            schedulerProvider.Scheduler.AdvanceBy(1);

            useCase.Verify(x => x.Invoke(), Times.Once);

            CollectionAssert.AreEqual(viewModel.Threads, messages);
        }

        #endregion

        #region ItemTappedCommand

        [Test]
        public void ItemTappedCommand()
        {
            var schedulerProvider = new SchedulerProvider();
            var message = ModelFixture.Message();
            var navigationService = Mock.NavigationService();
            var useCase = new Mock<IFetchDirectMessageThreadUseCase>();
            var viewModel = new InboxPageViewModel(navigationService.Object)
            {
                FetchDirectMessageThreadUseCase = useCase.Object,
                SchedulerProvider = schedulerProvider
            };

            viewModel.ItemTappedCommand.Execute(message);

            navigationService
                .Verify(
                    x =>
                        x.NavigateAsync(
                            "DirectMessagePage",
                            It.Is<INavigationParameters>(v => v[ParameterNames.Model] == message),
                            null,
                            true),
                    Times.Once());
        }
        #endregion
    }
}
