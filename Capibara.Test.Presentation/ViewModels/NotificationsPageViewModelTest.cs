using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Unit = System.Reactive.Unit;

using Moq;
using NUnit.Framework;

namespace Capibara.Presentation.ViewModels
{
    public class NotificationsPageViewModelTest
    {
        #region RefreshCommand

        [Test]
        public void RefreshCommand_WhenSuccess()
        {
            var schedulerProvider = new SchedulerProvider();
            var notifications = ModelFixture.NotificationCollection();
            var useCase = new Mock<IFetchNotificationsUseCase>();
            var viewModal = new NotificationsPageViewModel
            {
                SchedulerProvider = schedulerProvider,
                FetchNotificationsUseCase = useCase.Object
            };

            useCase.Setup(x => x.Invoke()).ReturnsObservable(notifications);

            viewModal.RefreshCommand.Execute();

            schedulerProvider.Scheduler.AdvanceBy(2);

            useCase.Verify(x => x.Invoke(), Times.Once);

            CollectionAssert.AreEqual(viewModal.Notifications, notifications);
        }

        #endregion


        #region ItemTappedCommand

        [Test]
        public void ItemTappedCommand()
        {
            var schedulerProvider = new SchedulerProvider();
            var notification = ModelFixture.Notification();
            var navigationService = Mock.NavigationService();
            var viewModel = new NotificationsPageViewModel(navigationService.Object)
            {
                SchedulerProvider = schedulerProvider
            };

            viewModel.ItemTappedCommand.Execute(notification);

            navigationService
                .Verify(
                    x =>
                        x.NavigateAsync(
                            "WebViewPage",
                            It.Is<INavigationParameters>(v =>
                                    v.Count == 2
                                &&  v[ParameterNames.Url].Equals(notification.Url)
                                &&  v[ParameterNames.Title].Equals("お知らせ")),
                            null,
                            true),
                    Times.Once());
        }
        #endregion
    }
}
