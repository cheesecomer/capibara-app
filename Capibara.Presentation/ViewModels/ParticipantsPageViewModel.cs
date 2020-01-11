using System;
using System.Linq;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

using Capibara.Domain.Models;
using Capibara.Domain.UseCases;
using Capibara.Presentation.Navigation;

using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class ParticipantsPageViewModel : ViewModelBase<Room>
    {
        [Unity.Dependency]
        public IGetCurrentUserUseCase GetCurrentUserUseCase { get; set; }

        [Unity.Dependency]
        public IFetchRoomParticipantsUseCase FetchRoomParticipantsUseCase { get; set; }

        public ReadOnlyReactiveCollection<User> Participants { get; }

        public AsyncReactiveCommand<User> ItemTappedCommand { get; }

        public AsyncReactiveCommand RefreshCommand { get; }

        public ParticipantsPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            Room model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Participants = this.Model
                .Participants
                .ToReadOnlyReactiveCollection()
                .AddTo(this.Disposable);

            this.RefreshCommand = new AsyncReactiveCommand()
                .WithSubscribe(this.FetchRoomParticipantsAsync)
                .AddTo(this.Disposable);

            this.ItemTappedCommand = new AsyncReactiveCommand<User>()
                .WithSubscribe(this.NavigateToProfilePageAsync)
                .AddTo(this.Disposable);
        }

        private Task FetchRoomParticipantsAsync()
        {
            return Observable
                .Defer(() => this.FetchRoomParticipantsUseCase.Invoke(this.Model))
                .SubscribeOn(this.SchedulerProvider.IO)
                .WithProgress(this.ProgressDialogService)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Select(_ => Unit.Default)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private Task NavigateToProfilePageAsync(User user)
        {
            return Observable
                .Defer(this.GetCurrentUserUseCase.Invoke)
                .SubscribeOn(SchedulerProvider.IO)
                .Select(currentUser => currentUser.Id == user.Id
                    ? new Pair<string, User>("MyProfilePage", currentUser)
                    : new Pair<string, User>("UserProfilePage", user))
                .SelectMany(x =>
                {
                    var parameters = new NavigationParameterBuilder { Model = x.Second }.Build();
                    return this.NavigationService.NavigateAsync(x.First, parameters);
                })
                .SubscribeOn(SchedulerProvider.UI)
                .ToTask();
        }
    }
}
