using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.OS;
using Android.Content;
using Android.Gms.Ads;

using Capibara.Forms;
using Capibara.Droid.Renderers;

using Java.Lang;

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

                var extras = new Bundle();
                extras.PutString("max_ad_content_rating", "T");
                var request = new AdRequest.Builder()
                    .AddNetworkExtrasBundle(
                        Class.FromType(typeof(Google.Ads.Mediation.Admob.AdMobAdapter)),
                        extras)
                    .Build();

                adMobBanner.LoadAd(request);

                this.SetNativeControl(adMobBanner);
            }
        }

        private class AdListener : Android.Gms.Ads.AdListener
        {
            private readonly AdView adView;

            public AdListener(AdView adView)
            {
                this.adView = adView;
            }

            public override void OnAdFailedToLoad(int errorCode)
            {
                base.OnAdFailedToLoad(errorCode);

                System.Threading.Tasks.Task.Delay(5000).ContinueWith(_ =>
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var extras = new Bundle();
                        extras.PutString("max_ad_content_rating", "T");
                        var request = new AdRequest.Builder()
                            .AddNetworkExtrasBundle(
                                Class.FromType(typeof(Google.Ads.Mediation.Admob.AdMobAdapter)),
                                extras)
                            .Build();

                        this.adView.LoadAd(request);
                    });
                });
            }
        }
    }
}
