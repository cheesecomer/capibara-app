using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class FloorMapPageViewModel : ViewModelBase
    {
        public FloorMapPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.RefreshCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.RefreshCommand.Subscribe(this.RefreshAsync);

            this.ItemTappedCommand = new AsyncReactiveCommand<Room>().AddTo(this.Disposable);
            this.ItemTappedCommand.Subscribe(this.ItemTapped);
        }

        public ReactiveCollection<Room> Rooms { get; } = new ReactiveCollection<Room>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<Room> ItemTappedCommand { get; }

        [Unity.Dependency]
        public IFetchRoomsUseCase FetchRoomsUseCase { get; set; }

        private Task ItemTapped(Room item)
        {
            var parameters = new NavigationParameters { { ParameterNames.Model, item } };
            return this.NavigationService.NavigateAsync("RoomPage", parameters);
        }

        private Task RefreshAsync()
        {
            return Observable
                .FromAsync(
                    this.FetchRoomsUseCase.Invoke,
                    this.SchedulerProvider.IO)
                .ObserveOn(this.SchedulerProvider.UI)
                .Do(rooms =>
                {
                    rooms.ForEach(x => {
                        var room = this.Rooms.FirstOrDefault(y => y.Id == x.Id);
                        if (room != null)
                        {
                            room.Restore(x);
                        }
                        else
                        {
                            this.Rooms.Add(x);
                        }
                    });
                })
                .Select(_ => Unit.Default)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }
    }
}
