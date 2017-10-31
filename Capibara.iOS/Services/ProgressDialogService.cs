using System;
using System.Diagnostics;

using UIKit;
using CoreGraphics;

using System.Threading.Tasks;

using Capibara.Services;

namespace Capibara.iOS.Services
{
    public class ProgressDialogService : IProgressDialogService
    {
        public async Task DisplayAlertAsync(Task task, string message = null)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            var rootViewController = window.RootViewController;
            while (rootViewController.PresentedViewController != null)
            {
                rootViewController = rootViewController.PresentedViewController;
            }

            var bounds = UIScreen.MainScreen.Bounds;

            var container = new UIView(bounds)
            {
                BackgroundColor = UIColor.Black,
                Alpha = 0.75f,
                AutoresizingMask = UIViewAutoresizing.All,
            };

            rootViewController.Add(container);

            nfloat labelHeight = 22;
            nfloat labelWidth = bounds.Width - 20;

            // derive the center x and y
            nfloat centerX = bounds.Width / 2;
            nfloat centerY = bounds.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            var activitySpinner = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge)
            {
                AutoresizingMask = UIViewAutoresizing.All,
            };

            activitySpinner.Frame = new CGRect(
                centerX - (activitySpinner.Frame.Width / 2),
                centerY - activitySpinner.Frame.Height - 20,
                activitySpinner.Frame.Width,
                activitySpinner.Frame.Height);

            container.AddSubview(activitySpinner);

            activitySpinner.StartAnimating();

            // create and configure the "Loading Data" label
            if (message.IsNullOrEmpty())
            {
                var loadingLabel = new UILabel(new CGRect(
                    centerX - (labelWidth / 2),
                    centerY + 20,
                    labelWidth,
                    labelHeight
                ));
                loadingLabel.BackgroundColor = UIColor.Clear;
                loadingLabel.TextColor = UIColor.White;
                loadingLabel.Text = message;
                loadingLabel.TextAlignment = UITextAlignment.Center;
                loadingLabel.AutoresizingMask = UIViewAutoresizing.All;
                container.AddSubview(loadingLabel);
            }

            try
            {
                await task;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            UIView.Animate(
                0.5, // duration
                () => { container.Alpha = 0; },
                () => { container.RemoveFromSuperview(); }
            );

            await Task.Delay(TimeSpan.FromSeconds(0.5));

            container.Dispose();
        }
    }
}
