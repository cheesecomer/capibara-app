using System;
using System.IO;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Capibara.Models;
using Capibara.Services;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

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
        }

        public async void OnDestroySuccess(object sender, EventArgs args)
        {
            await this.NavigationService.NavigateAsync("/SignInPage");
        }
    }
}
