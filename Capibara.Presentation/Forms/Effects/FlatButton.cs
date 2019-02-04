using System.Linq;
using Xamarin.Forms;

namespace Capibara.Presentation.Forms.Effects
{
    public class FlatButtonRoutingEffect : RoutingEffect
    {
        public FlatButtonRoutingEffect() : base("Capibara.Presentation.Forms.Effects." + nameof(FlatButton)) { }
    }

    public static class FlatButton
    {
        public static readonly BindableProperty EnableProperty =
            BindableProperty.CreateAttached(
                    "Enable",
                    typeof(bool),
                    typeof(FlatButton),
                    false,
                    propertyChanged: EnableChanged);

        public static readonly BindableProperty RippleColorProperty =
            BindableProperty.CreateAttached(
                    "RippleColor",
                    typeof(Color),
                    typeof(FlatButton),
                    default(Color));

        public static void SetEnable(BindableObject view, bool value)
        {
            view.SetValue(EnableProperty, value);
        }

        public static bool GetEnable(BindableObject view)
        {
            return (bool)view.GetValue(EnableProperty);
        }

        public static void SetRippleColor(BindableObject view, Color value)
        {
            view.SetValue(RippleColorProperty, value);
        }

        public static Color GetRippleColor(BindableObject view)
        {
            return (Color)view.GetValue(RippleColorProperty);
        }

        private static void EnableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is View view)
            {
                if (!(view is Button)) return;
                
                if ((bool)newValue)
                {
                    view.Effects.Add(new FlatButtonRoutingEffect());
                }
                else
                {
                    var toRemove = view.Effects.FirstOrDefault(e => e is FlatButtonRoutingEffect);
                    if (toRemove != null)
                        view.Effects.Remove(toRemove);
                }
            }
        }
    }
}
