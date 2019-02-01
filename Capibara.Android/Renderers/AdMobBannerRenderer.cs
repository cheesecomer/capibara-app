using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Content;
using Android.Gms.Ads;

using Capibara.Forms;
using Capibara.Droid.Renderers;

[assembly: ExportRenderer(typeof(AdMobBanner), typeof(AdMobBannerRenderer))]
namespace Capibara.Droid.Renderers
{
    public class AdMobBannerRenderer : ViewRenderer<AdMobBanner, AdView>
    {
        public AdMobBannerRenderer(Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<AdMobBanner> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var adMobBanner = new AdView(this.Context)
                {
                    AdUnitId = PlatformVariable.AdMobUnitIdForBanner,
                    AdSize = AdSize.Banner,
                };

                adMobBanner.LoadAd(new AdRequest.Builder().Build());

                this.SetNativeControl(adMobBanner);
            }
        }

        private class AdListener : Android.Gms.Ads.AdListener
        {
            private AdView adView;

            public AdListener(AdView adView)
            {
                this.adView = adView;
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                base.OnAdFailedToLoad(errorCode);

                System.Threading.Tasks.Task.Delay(5000).ContinueWith(_ =>
                {
                    Device.BeginInvokeOnMainThread(() => this.adView.LoadAd(new AdRequest.Builder().Build()));
                });
            }
        }
    }
}
