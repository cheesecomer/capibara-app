using System;
using System.Threading.Tasks;

using Capibara.Domain.Models;
using Capibara.Presentation.Navigation;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.Presentation.ViewModels
{
    public class MyProfilePageViewModel : UserViewModel
    {
        public AsyncReactiveCommand EditCommand { get; }

        public AsyncReactiveCommand SettingCommand { get; }

        public AsyncReactiveCommand ShowFriendsCommand { get; }

        public ReactiveProperty<int> FriendCount { get; }

        public MyProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.FriendCount = this.Model
                .ToReactivePropertyAsSynchronized(x => x.FriendsCount)
                .AddTo(this.Disposable);

            this.EditCommand = new AsyncReactiveCommand()
                .WithSubscribe(this.NavigateToEditProfilePageAsync)
                .AddTo(this.Disposable);

            this.SettingCommand = new AsyncReactiveCommand()
                .WithSubscribe(this.NavigateToSettingPageAsync)
                .AddTo(this.Disposable);

            this.ShowFriendsCommand = new AsyncReactiveCommand()
                .WithSubscribe(this.NavigateToFollowUsersPageAsync)
                .AddTo(this.Disposable);
        }

        private Task NavigateToEditProfilePageAsync()
        {
            var parameters = new NavigationParameterBuilder { Model = this.Model }.Build();
            return this.NavigationService.NavigateAsync("EditProfilePage", parameters);
        }

        private Task NavigateToSettingPageAsync()
            => this.NavigationService.NavigateAsync("SettingPage");

        private Task NavigateToFollowUsersPageAsync()
            => this.NavigationService.NavigateAsync("FollowUsersPage");
    }
}
