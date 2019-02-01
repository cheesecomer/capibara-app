using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Capibara.Forms;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LinkableLabel), typeof(Capibara.iOS.Renderers.LinkableLabelRenderer))]
namespace Capibara.iOS.Renderers
{
    public class LinkableLabelRenderer : LabelRenderer
    {

        readonly List<Match> Matches = new List<Match>();

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            Control.UserInteractionEnabled = true;
            Control.AddGestureRecognizer(new UITapGestureRecognizer(this.OnTapUrl));

            UpdateLinks();
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == nameof(this.Element.Text))
            {
                UpdateLinks();
            }
        }

        protected void UpdateLinks()
        {
            if (Element is LinkableLabel element)
            {
                if (Element.Text.IsNullOrEmpty()) return;

                var attributedString = new NSMutableAttributedString(Element.Text, Control.Font, Element.TextColor.ToUIColor());

                attributedString.AddAttribute(
                    UIStringAttributeKey.ForegroundColor,
                    Element.TextColor.Equals(Color.Default) ? Color.Black.ToUIColor() : Element.TextColor.ToUIColor(),
                    new NSRange(0, Element.Text.Length)
                );

                var matches = Regex.Matches(Element.Text, @"https?://[-_.!~*'a-zA-Z0-9;/?:@&=+$,%#]+", RegexOptions.IgnoreCase);

                foreach (Match match in matches)
                {
                    var range = new NSRange(match.Index, match.Length);
                    attributedString.AddAttribute(UIStringAttributeKey.ForegroundColor, element.LinkTextColor.ToUIColor(), range);
                    attributedString.AddAttribute(UIStringAttributeKey.UnderlineColor, element.LinkTextColor.ToUIColor(), range);
                    attributedString.AddAttribute(UIStringAttributeKey.UnderlineStyle, NSNumber.FromInt32((int)NSUnderlineStyle.Single), range);

                    this.Matches.Add(match);
                }

                Control.AttributedText = attributedString;
            }
        }

        protected void OnTapUrl(UIGestureRecognizer tap)
        {
            var location = tap.LocationInView(Control);
            var textView = new UITextView(Control.Frame);

            textView.TextContainer.LineFragmentPadding = 0;
            textView.TextContainerInset = UIEdgeInsets.Zero;
            textView.AttributedText = Control.AttributedText;

            var position = textView.GetOffsetFromPosition(
                textView.BeginningOfDocument,
                textView.GetClosestPositionToPoint(location)
            );

            var url = this.Matches.FirstOrDefault(m =>
                m.Index <= position && position <= (m.Index + m.Length)
            );

            if (url != null)
            {
                if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
                {
                    UIApplication.SharedApplication.OpenUrl(new Uri(url.Value), new NSDictionary { }, null);
                }
                else
                {
                    UIApplication.SharedApplication.OpenUrl(new Uri(url.Value));
                }
            }
        }
    }
}
