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

namespace Capibara.Presentation.ViewModels
{
    public class NotificationsPageViewModel : ViewModelBase
    {
        public IFetchNotificationsUseCase FetchNotificationsUseCase { get; set; }

        public ReactiveCollection<Notification> Notifications { get; } = new ReactiveCollection<Notification>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<Notification> ItemTappedCommand { get; }

        public NotificationsPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand =
                new AsyncReactiveCommand()
                    .WithSubscribe(this.RefreshAsync)
                    .AddTo(this.Disposable);

            this.ItemTappedCommand =
                new AsyncReactiveCommand<Notification>()
                    .WithSubscribe(this.NavigateAsync)
                    .AddTo(this.Disposable);
        }

        private Task RefreshAsync()
        {
            return
                Observable.Defer(FetchNotificationsUseCase.Invoke)
                .SubscribeOn(this.SchedulerProvider.IO)
                .Do(notifications =>
                {
                    this.Notifications.Clear();
                    notifications?.ForEach(this.Notifications.Add);
                })
                .SubscribeOn(this.SchedulerProvider.UI)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Select(_ => Unit.Default)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private Task NavigateAsync(Notification notification)
        {
            var parameters =
                new NavigationParameterBuilder { Url = notification.Url, Title = "お知らせ" }
                    .Build();

            return this.NavigationService.NavigateAsync("WebViewPage", parameters);
        }
    }
}
