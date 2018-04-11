using System;
using System.Linq;
using System.Threading.Tasks;

using Capibara.Services;

using UIKit;
using SafariServices;
using Foundation;

namespace Capibara.iOS.Services
{
    public class SnsLoginService : ISnsLoginService
    {
        private SFAuthenticationSession authSession;

        private IIsolatedStorage isolatedStorage;

        public SnsLoginService(IIsolatedStorage isolatedStorage)
        {
            this.isolatedStorage = isolatedStorage;
        }

        async void ISnsLoginService.Open(string url)
        {
            var uri = new Uri(url);
            if (UIDevice.CurrentDevice.CheckSystemVersion(11, 0))
            {
                this.authSession = new SFAuthenticationSession(uri, "com.cheesecomer.capibara", (callbackUrl, e) =>
                {
                    if (callbackUrl == null) return;
                    Xamarin.Forms.Device.OpenUri(callbackUrl);
                });

                this.authSession.Start();
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(9, 0))
            {
                var safariViewController = new SFSafariViewController(new Uri(url));

                var viewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
                while (viewController.PresentedViewController != null)
                {
                    viewController = viewController.PresentedViewController;
                }

                await viewController.PresentViewControllerAsync(safariViewController, true);
            }
            else
            {
                Xamarin.Forms.Device.OpenUri(uri);
            }
        }
    }
}
