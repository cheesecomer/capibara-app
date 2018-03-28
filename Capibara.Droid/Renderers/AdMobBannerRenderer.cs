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
                var adMobBanner = new AdView(this.Context);
                adMobBanner.AdSize = AdSize.Banner;
                adMobBanner.AdUnitId = PlatformVariable.AdMobUnitIdForBanner;

                var requestbuilder = new AdRequest.Builder();
                //requestbuilder.te
                adMobBanner.LoadAd(requestbuilder.Build());

                this.SetNativeControl(adMobBanner);
            }
        }
    }
}
