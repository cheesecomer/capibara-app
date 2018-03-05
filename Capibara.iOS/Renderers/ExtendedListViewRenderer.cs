using System;
using System.Linq;

using CoreGraphics;

using Capibara.Forms;
using Capibara.iOS.Renderers;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedListView), typeof(ExtendedListViewRenderer))]
namespace Capibara.iOS.Renderers
{
    public class ExtendedListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            this.Control.Transform = new CGAffineTransform(1, 0, 0, -1, 0, 0);
        }
        
    }
}
