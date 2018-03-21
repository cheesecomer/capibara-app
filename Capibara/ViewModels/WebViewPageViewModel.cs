using Prism.Navigation;
using Prism.Services;

using Reactive.Bindings;

using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class WebViewPageViewModel : ViewModelBase
    {
        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<WebViewSource> Source { get; } = new ReactiveProperty<WebViewSource>();

        public WebViewPageViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null)
            : base(navigationService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            this.Title.Value = parameters.TryGetValue<string>(ParameterNames.Title);

            var url = parameters.TryGetValue<string>(ParameterNames.Url);
            if (url.IsPresent())
            {
                this.Source.Value = new UrlWebViewSource { Url = url };
                this.Title.Value = this.Title.Value.Presence() ?? url;
            }
        }
    }
}
