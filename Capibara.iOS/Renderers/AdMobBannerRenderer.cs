using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Google.MobileAds;
using UIKit;
using CoreGraphics;

using Capibara.Forms;
using Capibara.iOS.Renderers;

[assembly: ExportRenderer(typeof(AdMobBanner), typeof(AdMobBannerRenderer))]
namespace Capibara.iOS.Renderers
{
    public class AdMobBannerRenderer : ViewRenderer
    {
        private BannerView adMobBanner;

        private bool viewOnScreen;

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement == null)
                return;

            if (e.OldElement == null)
            {
                this.adMobBanner = new BannerView(AdSizeCons.Banner, new CGPoint(-10, 0))
                {
                    AdUnitID = PlatformVariable.AdMobUnitIdForBanner,
                    RootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController
                };

                this.adMobBanner.AdReceived += (sender, args) =>
                {
                    if (!viewOnScreen) AddSubview(adMobBanner);
                    viewOnScreen = true;
                };

                this.adMobBanner.ReceiveAdFailed += (sender, args) =>
                {
                    System.Threading.Tasks.Task.Delay(1000).ContinueWith(t => this.DoAdRequest());
                };

                this.DoAdRequest();
                this.SetNativeControl(this.adMobBanner);
            }
        }

        private void DoAdRequest()
        {
            var request = Request.GetDefaultRequest();
            request.TestDevices = new[] { "kGADSimulatorID" };
            this.adMobBanner.LoadRequest(request);
        }
    }
}
