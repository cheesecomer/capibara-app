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

        public MyProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            // EditCommand
            this.EditCommand = new AsyncReactiveCommand().AddTo(this.Disposable);
            this.EditCommand.Subscribe(async () => {
                var parameters = new NavigationParameters { { ParameterNames.Model, this.Model } };
                await this.NavigationService.NavigateAsync("EditProfilePage", parameters);
            });
        }
    }
}
