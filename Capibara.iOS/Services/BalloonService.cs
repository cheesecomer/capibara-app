using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Capibara.Services;

using CoreGraphics;
using Foundation;
using UIKit;

namespace Capibara.iOS.Services
{
    public class BalloonService : IBalloonService
    {
        private Queue<string> messageQueue = new Queue<string>();

        void IBalloonService.DisplayBalloon(string message)
        {
            lock(messageQueue)
            {
                messageQueue.Enqueue(message);
            }

            if (messageQueue.Count != 1) return;

            Xamarin.Forms.Device.BeginInvokeOnMainThread(this.DoDisplayBalloon);
        }

        private async void DoDisplayBalloon()
        {
            while (messageQueue.Count > 0)
            {
                if (UIApplication.SharedApplication.KeyWindow.IsNull())
                {
                    lock (messageQueue) messageQueue.Clear();
                    return;
                }


                var message = messageQueue.Peek();

                var screenBounds = UIScreen.MainScreen.Bounds;
                var containerPosY = (screenBounds.Height.Equals(2436) ? 44 : 20) + 44 + 25;
                var containerWidth = screenBounds.Width - 100;

                var messageLabel = new UILabel { Text = message, TextAlignment = UITextAlignment.Center, TextColor = UIColor.White, Lines = 0 };
                var requestHeight = messageLabel.SizeThatFits(new CGSize(containerWidth - 10, double.MaxValue)).Height;
                var size = new CGSize(containerWidth, Math.Max(requestHeight + 10f, 25f));

                messageLabel.Frame = new CGRect(new CGPoint(5, 0), new CGSize(containerWidth - 10, size.Height));

                var container = new UIView(new CGRect(new CGPoint(screenBounds.Width, containerPosY), size))
                    {
                        BackgroundColor = UIColor.FromRGBA(0x31, 0x42, 0x57, (int)(0xff * 0.9f)),
                        ClipsToBounds = true
                    };
                container.Layer.CornerRadius = 12.5f;

                container.AddSubview(messageLabel);

                UIApplication.SharedApplication.KeyWindow.AddSubview(container);
                    
                UIView.Animate(
                    0.25,
                    0,
                    UIViewAnimationOptions.CurveEaseOut,
                    () => container.Frame = new CGRect(new CGPoint(50, containerPosY), size),
                    () => { }
                );
                await Task.Delay(250);

                await Task.Delay(2000);

                UIView.Animate(
                    0.25,
                    0,
                    UIViewAnimationOptions.CurveEaseOut,
                    () => container.Frame = new CGRect(new CGPoint(-screenBounds.Width, containerPosY), size),
                    () => { }
                );

                await Task.Delay(500);

                container.RemoveFromSuperview();
                container.Dispose();

                lock (messageQueue) messageQueue.Dequeue();
            }
        }
    }
}
