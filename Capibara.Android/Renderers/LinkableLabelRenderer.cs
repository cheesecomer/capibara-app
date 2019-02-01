using Android.Text.Util;
using Capibara.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LinkableLabel), typeof(Capibara.Droid.Renderers.LinkableLabelRenderer))]
namespace Capibara.Droid.Renderers
{
    public class LinkableLabelRenderer : LabelRenderer
    {
        public LinkableLabelRenderer(Android.Content.Context context) : base(context) { }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            var element = Element as LinkableLabel;

            Linkify.AddLinks(this.Control, MatchOptions.WebUrls);
            this.Control.SetLinkTextColor(element.LinkTextColor.ToAndroid());
        }
    }
}
