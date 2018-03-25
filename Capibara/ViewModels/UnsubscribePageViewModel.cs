using System;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;

namespace Capibara.ViewModels
{
    public class UnsubscribePageViewModel : ViewModelBase
    {
        public AsyncReactiveCommand UnsubscribeCommand { get; }

        public UnsubscribePageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base (navigationService, pageDialogService)
        {
            this.UnsubscribeCommand = new AsyncReactiveCommand();
            this.UnsubscribeCommand.Subscribe(async x => await this.ProgressDialogService.DisplayProgressAsync(this.CurrentUser.Destroy()));
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.CurrentUser.DestroySuccess += this.OnDestroySuccess;
            this.CurrentUser.DestroyFail += this.OnFail;
        }

        public async void OnDestroySuccess(object sender, EventArgs args)
        {
            await this.NavigationService.NavigateAsync("/SignUpPage");
        }
    }
}
