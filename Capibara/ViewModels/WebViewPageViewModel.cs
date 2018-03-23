using System;

using Capibara.Services;

using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class WebViewPageViewModel : ViewModelBase
    {
        [Unity.Attributes.Dependency]
        public IOverrideUrlService OverrideUrlService { get; set; }

        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<UrlWebViewSource> Source { get; } = new ReactiveProperty<UrlWebViewSource>();

        public ReactiveCommand<IOverrideUrlCommandParameters> OverrideUrlCommand { get; } = 
            new ReactiveCommand<IOverrideUrlCommandParameters>();

        public WebViewPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService) { }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            this.Title.Value = parameters.TryGetValue<string>(ParameterNames.Title);

            var url = parameters.TryGetValue<string>(ParameterNames.Url);
            if (url.IsPresent())
            {
                this.Source.Value = new UrlWebViewSource { Url = url };
                this.Title.Value = this.Title.Value.Presence() ?? url;

                this.OverrideUrlCommand.Subscribe(
                    this.OverrideUrlService.OverrideUrl(
                        this.DeviceService,
                        url));
            }
        }
    }
}
