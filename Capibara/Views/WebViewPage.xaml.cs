using System;
using System.Reactive.Linq;
using Reactive.Bindings.Extensions;
using Xamarin.Forms;

namespace Capibara.Views
{
    public partial class WebViewPage : ContentPage
    {
        public WebViewPage()
        {
            InitializeComponent();

            this.WebView
                .PropertyChangedAsObservable()
                .Where(x => x.PropertyName == nameof(WebView.Source))
                .Subscribe(async _ => 
                {
                    this.ProgressBar.Opacity = 1;
                    this.ProgressBar.Progress = 0;
                    await this.ProgressBar.ProgressTo(0.9, 3500, Easing.CubicOut);
                });

            this.WebView.Navigated += (s, e) =>
            {
                this.ProgressBar.Opacity = 0;
                this.ProgressBar.Progress = 1;
            };
        }
    }
}
