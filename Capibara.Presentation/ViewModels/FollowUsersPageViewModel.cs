using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

using Capibara.Domain.UseCases;
using Capibara.Domain.Models;
using Capibara.Presentation.Navigation;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class FollowUsersPageViewModel : ViewModelBase
    {
        [Unity.Dependency]
        public IFetchFollowUsersUseCase FetchFollowUsersUseCase { get; set; }

        public ReactiveCollection<User> FollowUsers { get; } = new ReactiveCollection<User>();

        public AsyncReactiveCommand RefreshCommand { get; }

        public AsyncReactiveCommand<User> ItemTappedCommand { get; }

        public FollowUsersPageViewModel(
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
                new AsyncReactiveCommand<User>()
                .WithSubscribe(this.NavigateToProfilePageAsync)
                .AddTo(this.Disposable);
        }

        private Task RefreshAsync()
        {
            return Observable
                .Defer(this.FetchFollowUsersUseCase.Invoke)
                .Do(follows =>
                {
                    follows.ForEach(x =>
                    {
                        var user = this.FollowUsers.FirstOrDefault(y => y.Id == x.Id);
                        if (user != null)
                        {
                            user.Restore(x);
                        }
                        else
                        {
                            this.FollowUsers.Add(x);
                        }
                    });
                })
                .SubscribeOn(this.SchedulerProvider.UI)
                .ObserveOn(this.SchedulerProvider.UI)
                .WithProgress(this.ProgressDialogService)
                .Select(_ => Unit.Default)
                .RetryWhen(this.PageDialogService, this.SchedulerProvider.UI)
                .Catch(Observable.Return(Unit.Default))
                .ToTask();
        }

        private Task NavigateToProfilePageAsync(User user)
        {
            var parameters = new NavigationParameters { { ParameterNames.Model, user } };
            return this.NavigationService.NavigateAsync("UserProfilePage", parameters);
        }
    }
}
