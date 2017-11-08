using Capibara.iOS.Effects;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;


[assembly: ResolutionGroupName("Effects")]
[assembly: ExportEffect(typeof(BorderEffect), "BorderEffect")]
namespace Capibara.iOS.Effects
{
    public class BorderEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
            Control.Layer.BorderWidth = 0.6f;
            Control.Layer.BorderColor = Color.FromRgb(209, 209, 209).ToCGColor();
            Control.Layer.CornerRadius = 5;
        }

        protected override void OnDetached()
        {
            Control.Layer.BorderWidth = 0.0f;
        }
    }
}
