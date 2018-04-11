using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

using Capibara.Models;

using Prism.Services;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class FloorMapPageViewModel : ViewModelBase
    {
        public ReactiveCollection<Room> Rooms { get; } = new ReactiveCollection<Room>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<Room> ItemTappedCommand { get; }

        public FloorMapPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            // RefreshCommand
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(() => this.ProgressDialogService.DisplayProgressAsync(this.Refresh()));

            this.ItemTappedCommand = new AsyncReactiveCommand<Room>();
            this.ItemTappedCommand.Subscribe(async x =>
            {
                var parameters = new NavigationParameters { { ParameterNames.Model, x } };
                await this.NavigationService.NavigateAsync("RoomPage", parameters);
            });
        }

        private async Task Refresh()
        {
            var request = this.RequestFactory.RoomsIndexRequest().BuildUp(this.Container);
            try
            {
                var response = await request.Execute();
                response.Rooms?.ForEach(x => {
                    if (this.Rooms.Any(y => y.Id == x.Id))
                    {
                        this.Rooms.First(y => y.Id == x.Id).Restore(x);
                    }
                    else
                    {
                        this.Rooms.Add(x);
                    }
                });

            }
            catch (Exception e)
            {
                await this.DisplayErrorAlertAsync(e, () => this.Refresh());
            }
        }
    }
}
