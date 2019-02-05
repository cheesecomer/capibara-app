using System;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using Capibara.Services;
using Prism.Navigation;
using Prism.Services;
using Reactive.Bindings;
using Xamarin.Forms;

namespace Capibara.ViewModels
{
    public class OgpViewModel : ViewModelBase
    {
        public string Url { get; }

        public ReactiveCommand OpenUrlCommand { get; } = new ReactiveCommand();

        public AsyncReactiveCommand RefreshCommand { get; }

        public ReactiveProperty<bool> IsLoaded { get; } = new ReactiveProperty<bool>(false);

        public ReactiveProperty<string> Title { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<string> Description { get; } = new ReactiveProperty<string>();

        public ReactiveProperty<ImageSource> Image { get; } = new ReactiveProperty<ImageSource>();

        [Unity.Attributes.Dependency]
        public IBrowsingContextFactory BrowsingContextFactory { get; set; }

        public OgpViewModel(
            INavigationService navigationService = null,
            IPageDialogService pageDialogService = null,
            string url = null) : base(navigationService, pageDialogService)
        {
            this.Url = url;

            this.Title.Value = this.Url.ToLower();
            this.OpenUrlCommand.Subscribe(_ => this.DeviceService.OpenUri(new Uri(this.Url)));

            this.RefreshCommand = this.IsLoaded.Select(x => !x).ToAsyncReactiveCommand();
            this.RefreshCommand.Subscribe(this.DoRefresh);
        }

        protected override void OnContainerChanged()
        {
            base.OnContainerChanged();

            this.RefreshCommand.Execute();
        }

        private async Task DoRefresh()
        {
            var config = Configuration.Default.WithDefaultLoader();
            var document = await (this.BrowsingContextFactory?.Create(config).OpenAsync(this.Url) ?? Task.FromResult<IDocument>(null));
            if (document?.StatusCode != HttpStatusCode.OK)
            {
                return;
            }

            this.Title.Value = document.Title.Presence() ?? this.Url;
            var titleElement = document.QuerySelectorAll("meta[property=\"og:title\"]")?.FirstOrDefault();
            if (titleElement != null)
            {
                this.Title.Value = titleElement.GetAttribute("content");
            }

            var descriptionElement = document.QuerySelectorAll("meta[property=\"og:description\"]")?.FirstOrDefault();
            if (descriptionElement != null)
            {
                this.Description.Value = descriptionElement.GetAttribute("content");
            }

            var imageElement = document.QuerySelectorAll("meta[property=\"og:image\"]")?.FirstOrDefault();
            if (imageElement != null)
            {
                this.Image.Value = this.ImageSourceFactory.FromUri(new Uri(imageElement.GetAttribute("content")));
            }

            this.IsLoaded.Value = true;
        }
    }
}
