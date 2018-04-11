using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Airbnb.Lottie;

using Capibara.Services;

using CoreGraphics;
using Foundation;
using UIKit;

namespace Capibara.iOS.Services
{
    public class ProgressDialogService : IProgressDialogService
    {
        public async Task DisplayProgressAsync(Task task, string message = null)
        {
            var window = UIApplication.SharedApplication.KeyWindow;
            if (window.IsNull())
            {
                try
                {
                    await task;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

                return;
            }

            var bounds = UIScreen.MainScreen.Bounds;
            var centerX = bounds.Width / 2;
            var centerY = bounds.Height / 2 - 50;

            var container = new UIView(bounds)
            {
                BackgroundColor = UIColor.FromRGB(0x2f, 0x8e, 0x5b),
                Alpha = 0.75f,
                AutoresizingMask = UIViewAutoresizing.All,
            };

            window.AddSubview(container);

            var animationSize = new[] { bounds.Width, bounds.Height }.Min();
            var animationHalfSize = animationSize / 2;

            var animationView = new LOTAnimationView(NSUrl.FromFilename("loading.json"))
            {
                AutoresizingMask = UIViewAutoresizing.All,
                ContentMode = UIViewContentMode.ScaleAspectFit,
                LoopAnimation = true,
                Frame = new CGRect(
                    centerX - animationHalfSize,
                    centerY - animationHalfSize,
                    animationSize,
                    animationSize)
            };

            container.AddSubview(animationView);

            animationView.Play();

            // create and configure the "Loading Data" label
            //if (message.IsPresent())
            {
                nfloat labelHeight = 22;
                nfloat labelWidth = bounds.Width - 20;
                var loadingLabel = new UILabel(new CGRect(
                    centerX - (labelWidth / 2),
                    centerY + animationHalfSize - 50,
                    labelWidth,
                    labelHeight
                ));
                loadingLabel.BackgroundColor = UIColor.Clear;
                loadingLabel.TextColor = UIColor.White;
                loadingLabel.Text = "Loading Data";
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
                () => container.Alpha = 0,
                () => container.RemoveFromSuperview()
            );

            await Task.Delay(TimeSpan.FromSeconds(0.5));

            container.RemoveFromSuperview();
            container.Dispose();
        }
    }
}
