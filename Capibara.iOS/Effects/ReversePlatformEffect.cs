using System;
using System.Linq;
using CoreGraphics;
using Capibara.Effects;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportEffect(typeof(Capibara.iOS.Effects.ReversePlatformEffect), nameof(Reverse))]
namespace Capibara.iOS.Effects
{
    public class ReversePlatformEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            (Control ?? Container).Transform = new CGAffineTransform(1, 0, 0, -1, 0, 0);
        }

        protected override void OnDetached()
        {
            (Control ?? Container).Transform = new CGAffineTransform(1, 0, 0, 0, 0, 0);
        }
    }
}
