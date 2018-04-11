using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;

using Capibara.Models;

using Prism.Services;
using Prism.Navigation;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class AboutPageViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Version { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Copyright { get; } = new ReactiveProperty<string>();

        public AsyncReactiveCommand CloseCommand { get; }

        public AboutPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
            this.CloseCommand = new AsyncReactiveCommand();
            this.CloseCommand.Subscribe(this.NavigationService.GoBackAsync);

            this.Copyright.Value = $"Copyright © 2018-{DateTime.Now.Year} @cheese_comer.";
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.Version.Value = $"Version {this.ApplicationService.AppVersion}";
        }
    }
}
