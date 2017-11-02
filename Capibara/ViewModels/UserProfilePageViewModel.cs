using System;
using System.Reactive.Linq;

using Capibara.Models;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class UserProfilePageViewModel : ViewModelBase<User>
    {
        public ReactiveProperty<string> Nickname { get; }

        public ReactiveProperty<string> Biography { get; }

        public UserProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            User model = null)
            : base(navigationService, pageDialogService, model)
        {
            this.Nickname = this.Model
                .ObserveProperty(x => x.Nickname)
                .ToReactiveProperty();

            this.Biography = this.Model
                .ObserveProperty(x => x.Biography)
                .ToReactiveProperty();
        }
    }
}
