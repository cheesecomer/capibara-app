using System;
using System.Reactive.Linq;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Capibara.ViewModels
{
    public class MyProfilePageViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Nickname { get; } = new ReactiveProperty<string>();

        public MyProfilePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.CurrentUser.ObserveProperty(x => x.Nickname).Subscribe(x => this.Nickname.Value = x);
        }
    }
}
