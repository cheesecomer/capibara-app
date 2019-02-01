using System.Collections.Generic;
using System.Drawing;

using Capibara.Forms;
using Capibara.iOS.Renderers;

using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RoundImage), typeof(RoundImageRenderer))]
namespace Capibara.iOS.Renderers
{
    public class RoundImageRenderer : ImageRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            this.Control.Layer.MasksToBounds = false;
            this.Control.ClipsToBounds = true;

            this.SetupLayer();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            var layerPropertyNames = new List<string>
            {
                nameof(RoundImage.CornerRadius),
                nameof(RoundImage.BorderColor),
                nameof(RoundImage.BorderWidth),
                nameof(RoundImage.HasShadow)
            };

            if (layerPropertyNames.Exists(x => x == e.PropertyName))
            {
                this.SetupLayer();
            }
        }

        private void SetupLayer()
        {
            var view = this.Element as RoundImage;
            if (view == null) return;

            this.Control.Layer.CornerRadius = view.CornerRadius;
            this.Control.Layer.BorderColor = view.BorderColor.ToUIColor().CGColor;
            this.Control.Layer.BorderWidth = view.BorderWidth;

            if (view.HasShadow)
            {
                this.Layer.ShadowRadius = 5;
                this.Layer.ShadowColor = UIColor.Black.CGColor;
                this.Layer.ShadowOpacity = 0.8f;
                this.Layer.ShadowOffset = new SizeF();
            }
            else
            {
                this.Layer.ShadowOpacity = 0;
            }
        }
    }
}
