using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;

using Capibara.Models;

using Prism.Services;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class FloorMapPageViewModel : ViewModelBase<FloorMap>
    {
        public ReadOnlyReactiveCollection<Room> Rooms { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<Room> ItemTappedCommand { get; }

        public FloorMapPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            FloorMap model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Rooms =
                this.Model.Rooms.ToReadOnlyReactiveCollection();

            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayAlertAsync(this.Model.Refresh()));

            this.ItemTappedCommand = new AsyncReactiveCommand<Room>();
            this.ItemTappedCommand.Subscribe(async x =>
            {
                var parameters = new NavigationParameters();
                parameters.Add(ParameterNames.Model, x);
                await this.NavigationService.NavigateAsync("RoomPage", parameters);
            });

            this.Model.RefreshFail += this.OnRefreshFail;
        }

        private async void OnRefreshFail(object sender, Exception exception)
        {
            if (exception is Net.HttpUnauthorizedException)
            {
                await this.PageDialogService.DisplayAlertAsync("なんてこった！", "再度ログインしてください", "閉じる");

                await this.NavigationService.NavigateAsync("/SignInPage");
            }
        }
    }
}
