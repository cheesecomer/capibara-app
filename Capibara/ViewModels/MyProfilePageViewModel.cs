using Capibara.Models;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class MyProfilePageViewModel : UserViewModel
    {
        public AsyncReactiveCommand EditCommand { get; }

        public AsyncReactiveCommand SettingCommand { get; }

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

            // EditCommand
            this.EditCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.EditCommand.Subscribe(async () => {
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                await this.NavigationService.NavigateAsync("EditProfilePage", parameters);
            });

            this.SettingCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.SettingCommand.Subscribe(() => this.NavigationService.NavigateAsync("SettingPage"));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.Model.Restore(this.CurrentUser);
        }
    }
}
